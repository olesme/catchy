namespace Catchy.Sdk
{
    public static partial class QuantifiedModeChecks
    {
        /// <summary>
        /// Builds a single <see cref="CheckOperation"/> that applies <paramref name="makeCheck"/>
        /// to every item in the collection and evaluates the result against <paramref name="mode"/>.
        ///
        /// Execution strategy:
        ///   • Any async check  → always parallel (WhenAll)
        ///   • All sync + count ≥ SyncParallelThreshold → parallel via Task.WhenAll
        ///   • All sync + count &lt; SyncParallelThreshold → sequential loop (no Task allocation)
        ///
        /// <c>FailBecause()</c> on each CheckOperation MUST be a pure formatting closure —
        /// it must NOT re-execute the check or access shared mutable state.
        /// All data for the message must be captured at CheckOperation creation time.
        /// </summary>
        public static CheckOperation Apply<T>(
            QuantifiedMode mode,
            IEnumerable<T>? collection,
            Func<T, CheckOperation> makeCheck,
            bool isSkipped,
            Func<bool> getFailFast,
            Func<int> getSyncParallelThreshold)
        {
            FailureDetail? detail = null;
            string failMsg() => BuildMessage(mode, detail);

            return CheckOperation.Async(async () =>
            {
                detail = null;

                if (collection is null)
                {
                    detail = FailureDetail.NullCollection(mode);
                    return false;
                }

                if (!collection.Any())
                {
                    return mode switch
                    {
                        QuantifiedMode.Each => true,   // vacuously true
                        QuantifiedMode.Any => false,  // nothing to satisfy
                        QuantifiedMode.None => true,   // vacuously true
                        _ => true
                    };
                }

                var failFastEffective = getFailFast();
                var threshold = getSyncParallelThreshold();
                var ops = BuildOps(collection, makeCheck);

                bool hasAsync = HasAnyAsync(ops);
                bool useParallel = hasAsync || collection.Count() >= threshold;

                bool pass;
                if (useParallel)
                    (pass, detail) = await RunParallelAsync(mode, collection, ops, failFastEffective)
                        .ConfigureAwait(false);
                else
                    (pass, detail) = RunSequential(mode, collection, ops, failFastEffective);

                return pass;
            },
failMsg,
            isSkipped);
        }

        private static CheckOperation[] BuildOps<T>(
            IEnumerable<T> collection,
            Func<T, CheckOperation> makeCheck)
        {
            var ops = new CheckOperation[collection.Count()];
            int i = 0;
            foreach (var item in collection)
            {
                ops[i++] = makeCheck(item);
            }
            return ops;
        }

        private static bool HasAnyAsync(CheckOperation[] ops)
        {
            foreach (var op in ops)
                if (op.IsAsync) return true;
            return false;
        }

        private static (bool pass, FailureDetail? detail) RunSequential<T>(
            QuantifiedMode mode,
            IEnumerable<T> collection,
            CheckOperation[] ops,
            bool failFast)
        {
            var failed = new List<ItemResult>();
            var passed = new List<ItemResult>();

            int i = 0;
            foreach (var item in collection)
            {
                bool ok = ops[i].PassesSync!();
                var itemResult = new ItemResult(i, ValueFormatter.Format(item),
                    ok ? null : ops[i].FailBecause());
                i++;

                if (ok)
                {
                    passed.Add(itemResult);

                    if (mode == QuantifiedMode.Any)
                        return (true, null);

                    if (mode == QuantifiedMode.None)
                        return (false, FailureDetail.NoneViolated(collection.Count(), [itemResult]));
                }
                else
                {
                    failed.Add(itemResult);

                    if (mode == QuantifiedMode.Each && failFast)
                        return (false, FailureDetail.EachPartial(
                            collection.Count(), failed, passed.Count > 0 ? passed : null));
                }
            }

            return Evaluate(mode, collection.Count(), failed, passed);
        }

        private static async Task<(bool pass, FailureDetail? detail)> RunParallelAsync<T>(
            QuantifiedMode mode,
            IEnumerable<T> collection,
            CheckOperation[] ops,
            bool failFast)
        {
            // Build tasks upfront — pure sync ops are wrapped in Task.FromResult
            // so WhenAll handles both uniformly.
            var tasks = new Task<bool>[collection.Count()];
            for (int i = 0; i < collection.Count(); i++)
            {
                var op = ops[i];
                tasks[i] = op.IsAsync
                    ? op.PassesAsync!()
                    : Task.FromResult(op.PassesSync!());
            }

            // Await all completions before reading any result.
            // This is the key guarantee: every op.FailBecause() is called
            // after its own async closure has finished writing to its local state.
            bool[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

            // Build results in index order — deterministic diff regardless of
            // which tasks finished first.
            var failed = new List<ItemResult>();
            var passed = new List<ItemResult>();

            for (int i = 0; i < collection.Count(); i++)
            {
                var item = new ItemResult(i, ValueFormatter.Format(collection.ElementAt(i)),
                    results[i] ? null : ops[i].FailBecause());

                if (results[i])
                {
                    passed.Add(item);

                    if (mode == QuantifiedMode.None)
                        return (false, FailureDetail.NoneViolated(collection.Count(), [item]));
                }
                else
                {
                    failed.Add(item);
                }
            }

            if (mode == QuantifiedMode.Any && passed.Count > 0)
                return (true, null);

            if (mode == QuantifiedMode.Each && failFast && failed.Count > 0)
                return (false, FailureDetail.EachPartial(
                    collection.Count(), failed, passed.Count > 0 ? passed : null));

            return Evaluate(mode, collection.Count(), failed, passed);
        }

        private static (bool pass, FailureDetail? detail) Evaluate(
            QuantifiedMode mode, int total,
            List<ItemResult> failed, List<ItemResult> passed)
            => mode switch
            {
                QuantifiedMode.Each when failed.Count > 0
                    => (false, FailureDetail.EachFull(total, failed, passed.Count > 0 ? passed : null)),
                QuantifiedMode.Any
                    => (false, FailureDetail.AnyFailed(total, failed)),
                _ => (true, null)
            };

        private static string BuildMessage(QuantifiedMode mode, FailureDetail? d)
        {
            if (d is null)
                return mode switch
                {
                    QuantifiedMode.Each => "Expected every item to satisfy the condition",
                    QuantifiedMode.Any => "Expected at least one item to satisfy the condition",
                    _ => "Expected no items to satisfy the condition",
                };

            if (d.WasNull)
                return "Expected a non-null collection";

            const int maxShown = 10;
            var sb = new System.Text.StringBuilder();

            switch (mode)
            {
                case QuantifiedMode.Each:
                    sb.AppendLine(
                        $"Expected every item to satisfy the condition, " +
                        $"but {d.Failed.Count()} of {d.Total} failed:");
                    sb.AppendLine();
                    AppendItems(sb, d.Failed, maxShown);
                    if (d.IsPartial)
                        sb.AppendLine(
                            "  (partial — set CollectionFailFast = false in settings for full diff)");
                    if (d.Passed != null && d.Passed.Any())
                    {
                        sb.AppendLine();
                        sb.Append($"  Passed ({d.Passed.Count()}): ");
                        sb.AppendLine(string.Join(", ",
                            d.Passed.Take(5).Select(x => $"[{x.Index}] {x.Value}")));
                    }
                    break;

                case QuantifiedMode.Any:
                    sb.AppendLine(
                        $"Expected at least one item to satisfy the condition, " +
                        $"but none of {d.Total} did:");
                    sb.AppendLine();
                    AppendItems(sb, d.Failed, maxShown);
                    break;

                case QuantifiedMode.None:
                    sb.AppendLine(
                        $"Expected no items to satisfy the condition, " +
                        $"but {d.Failed.Count()} did:");
                    sb.AppendLine();
                    foreach (var item in d.Failed.Take(maxShown))
                        sb.AppendLine($"  [{item.Index}] {item.Value}");
                    if (d.Failed.Count() > maxShown)
                        sb.AppendLine($"  … and {d.Failed.Count() - maxShown} more");
                    break;
            }

            return sb.ToString().TrimEnd();
        }

        private static void AppendItems(
            System.Text.StringBuilder sb, IEnumerable<ItemResult> items, int max)
        {
            foreach (var item in items.Take(max))
            {
                sb.Append($"  [{item.Index}] {item.Value}");
                if (item.Reason is not null)
                    sb.Append($"  ← {TruncateLine(item.Reason)}");
                sb.AppendLine();
            }
            if (items.Count() > max)
                sb.AppendLine($"  … and {items.Count() - max} more");
        }

        private static string TruncateLine(string s, int max = 120)
        {
            var nl = s.IndexOf('\n');
            var line = nl < 0 ? s : s.Substring(0, nl).TrimEnd('\r');
            return line.Length > max ? line.Substring(0, max) + "…" : line;
        }

        private sealed record ItemResult(int Index, string Value, string? Reason);

        private sealed class FailureDetail
        {
            public int Total { get; }
            public bool WasNull { get; }
            public bool IsPartial { get; }

            public IEnumerable<ItemResult> Failed { get; }
            public IEnumerable<ItemResult>? Passed { get; }

            private FailureDetail(
                int total, bool wasNull, bool isPartial,
                IEnumerable<ItemResult> failed,
                IEnumerable<ItemResult>? passed)
            {
                Total = total; WasNull = wasNull; IsPartial = isPartial;
                Failed = failed; Passed = passed;
            }

            public static FailureDetail NullCollection(QuantifiedMode _) =>
                new(0, wasNull: true, isPartial: false, [], null);

            public static FailureDetail NoneViolated(int total, IEnumerable<ItemResult> violators) =>
                new(total, false, false, violators, null);

            public static FailureDetail AnyFailed(int total, IEnumerable<ItemResult> failed) =>
                new(total, false, false, failed, null);

            public static FailureDetail EachFull(
                int total,
                IEnumerable<ItemResult> failed,
                IEnumerable<ItemResult>? passed) =>
                new(total, false, false, failed, passed);

            public static FailureDetail EachPartial(
                int total,
                IEnumerable<ItemResult> failed,
                IEnumerable<ItemResult>? passed) =>
                new(total, false, isPartial: true, failed, passed);
        }
    }
}
