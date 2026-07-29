using System.Text;
using Catchy.Sdk;

namespace Catchy
{
    public sealed class SoftState
    {
        private readonly List<AssertionException> _errors = [];
        public readonly object Lock = new();

        /// <summary>
        /// If true (default), FlushSilentlyIfNeeded returns null after first successful flush.
        /// Set to false to allow multiple flushes of the same errors.
        /// </summary>
        public bool FlushOnce { get; set; } = true;

        /// <summary>
        /// True after FlushSilentlyIfNeeded was called and returned non-null result.
        /// </summary>
        public bool AlreadyFlushed { get; set; }

        private int? _checkpoint;

        public bool HasFailures { get { lock (Lock) return _errors.Count > 0; } }
        public int ErrorCount { get { lock (Lock) return _errors.Count; } }
        public IReadOnlyList<AssertionException> Errors { get { lock (Lock) return [.. _errors]; } }

        public AggregateAssertionException? AggregateException => GetAggregateAssertionException();
        /// <summary>
        /// Called on auto-flush before FlushAction. Use for logging, screenshots, report attachments.
        /// </summary>
        public IReadOnlyList<Func<AggregateAssertionException, Task>> OnFlush { get; set; } = [];

        /// <summary>
        /// Called after notifiers. null = throw (default).
        /// Override to inject failure without throwing (e.g. Reqnroll ScenarioContext.InjectError).
        /// </summary>
        public Func<AggregateAssertionException, Task>? FlushAction { get; set; }

        internal void Capture(Exception ex)
        {
            var ae = ex as AssertionException ?? new AssertionException(ex.Message, ex);
            lock (Lock) _errors.Add(ae);
        }

        public void Clear() { lock (Lock) { _errors.Clear(); AlreadyFlushed = false; } }

        public int Checkpoint()
        {
            lock (Lock) { _checkpoint = _errors.Count; return _errors.Count; }
        }

        public void Revert(int to)
        {
            lock (Lock) { if (to < _errors.Count) _errors.RemoveRange(to, _errors.Count - to); }
        }

        public void Revert()
        {
            if (_checkpoint.HasValue) Revert(_checkpoint.Value);
        }

        private AggregateAssertionException? GetAggregateAssertionException(string? message = null)
        {
            lock (Lock)
            {
                if (_errors.Count == 0) return null;

                var builder = new StringBuilder(message);
                builder.AppendLine($"{_errors.Count} soft assertion{(_errors.Count == 1 ? "" : "s")} failed:");
                for (int i = 0; i < _errors.Count; i++)
                {
                    var error = _errors[i];
                    builder.AppendLine();
                    builder.AppendLine($"  [{i + 1}] {error.Body ?? error.Message}");
                    if (!string.IsNullOrEmpty(error.Chain))
                        builder.AppendLine($"      {error.Chain}");
                    if (error.AssertionSource is { } src)
                        builder.AppendLine($"      {Path.GetFileName(src.File)}:{src.Line} ({src.Member})");
                }

                return new AggregateAssertionException([.. _errors], builder.ToString().TrimEnd());
            }
        }

        public async Task FlushIfNeeded(bool ifNotAlready = true)
        {
            if (!HasFailures || (ifNotAlready && AlreadyFlushed)) return;

            var agg = GetAggregateAssertionException("[SOFT ASSERTIONS FLUSH]\n")!;
            AlreadyFlushed = true;
            var notifiers = OnFlush;
            for (int i = 0; i < notifiers.Count; i++)
            {
                try { await notifiers[i](agg).ConfigureAwait(false); }
                catch (Exception ex) { Console.Error.WriteLine($"[CATCHY OnFlush ERROR]: {ex.Message}"); }
            }

            if (FlushAction is not null)
                await FlushAction(agg).ConfigureAwait(false);
            else
                throw agg;
        }
    }

    namespace Sdk
    {
        public static class SoftStateExtensions
        {
            public static string? BuildMessage(this SoftState softState)
            {
                return BuildAggregateMessage(softState.Errors);
            }

            public static string? BuildAggregateMessage(IEnumerable<AssertionException> failures, SourceLocation? flushedAt = null, string? message = null)
            {
                var list = failures as IReadOnlyList<AssertionException> ?? failures.ToList();
                if (list.Count == 0) return null;
                bool allSameMember = list.All(f =>
                    f.AssertionSource?.Member == list[0].AssertionSource?.Member);
                var sb = new StringBuilder(message);
                sb.AppendLine($"{list.Count} soft assertion{(list.Count == 1 ? "" : "s")} failed:");
                for (int i = 0; i < list.Count; i++)
                {
                    var f = list[i];
                    sb.AppendLine();
                    sb.AppendLine($"  [{i + 1}] {f.Body ?? f.Message}");
                    if (!string.IsNullOrEmpty(f.Chain))
                        sb.AppendLine($"      {f.Chain}");
                    // Show per-item source only when items come from different members
                    if (!allSameMember && f.AssertionSource is { } src)
                        sb.AppendLine($"      {Path.GetFileName(src.File)}:{src.Line} ({src.Member})");
                }
                if (flushedAt is { } fa)
                {
                    sb.AppendLine();
                    sb.Append($"Flushed at: {Path.GetFileName(fa.File)}:{fa.Line} ({fa.Member})");
                }
                return sb.ToString().TrimEnd();
            }
        }
    }
}
