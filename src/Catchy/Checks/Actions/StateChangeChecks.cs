namespace Catchy.Sdk
{
    public static class StateChangeChecks
    {
        public static CheckOperation Changes<TState>(
            Func<Task> func, Func<TState> selector,
            TState? from, TState? to, bool hasFrom, bool hasTo,
            string? selectorExpr, string? fromExpr, string? toExpr,
            SlotContainer slots, bool isSkipped)
        {
            TState? before = default;
            TState? after = default;

            return CheckOperation.Async(async () =>
            {
                before = selector();

                if (hasFrom && !EqualityComparer<TState>.Default.Equals(before!, from!))
                    return false;

                var result = await FuncExecution.EnsureAsync(func, slots).ConfigureAwait(false);
                if (result.caught is not null) throw result.caught;

                after = selector();

                if (hasTo && !EqualityComparer<TState>.Default.Equals(after!, to!))
                    return false;

                return !EqualityComparer<TState>.Default.Equals(before!, after!);
            },
            () => BuildMessage(before, after, from, to, hasFrom, hasTo, selectorExpr, fromExpr, toExpr),
            isSkipped);
        }

        public static CheckOperation DoesNotChange<TState>(
            Func<Task> func, Func<TState> selector, string? selectorExpr,
            SlotContainer slots, bool isSkipped)
        {
            TState? before = default;
            TState? after = default;

            return CheckOperation.Async(async () =>
            {
                before = selector();
                var result = await FuncExecution.EnsureAsync(func, slots).ConfigureAwait(false);
                if (result.caught is not null) throw result.caught;
                after = selector();
                return EqualityComparer<TState>.Default.Equals(before!, after!);
            },
            () => $"Expected {selectorExpr ?? "<selector>"} not to change, but changed from {ValueFormatter.Format(before)} to {ValueFormatter.Format(after)}",
            isSkipped);
        }

        public static CheckOperation ChangesBy<TState>(
            Func<Task> func, Func<TState> selector, TState delta,
            string? selectorExpr, string? deltaExpr,
            SlotContainer slots, bool isSkipped)
            where TState : struct, IComparable<TState>
        {
            TState before = default;
            TState after = default;
            object? actualDelta = null;

            return CheckOperation.Async(async () =>
            {
                before = selector();
                var result = await FuncExecution.EnsureAsync(func, slots).ConfigureAwait(false);
                if (result.caught is not null) throw result.caught;
                after = selector();
                actualDelta = NumericOps.Subtract(after, before);
                return NumericOps.AreEqual(actualDelta, delta);
            },
            () => $"Expected {selectorExpr ?? "<selector>"} to change by {ExprFormat.Inline(delta, deltaExpr)}, but changed by {ValueFormatter.Format(actualDelta)} (from {ValueFormatter.Format(before)} to {ValueFormatter.Format(after)})",
            isSkipped);
        }

        public static CheckOperation Increments<TState>(
            Func<Task> func, Func<TState> selector, string? selectorExpr,
            SlotContainer slots, bool isSkipped)
            where TState : IComparable<TState>
        {
            TState? before = default;
            TState? after = default;

            return CheckOperation.Async(async () =>
            {
                before = selector();
                var result = await FuncExecution.EnsureAsync(func, slots).ConfigureAwait(false);
                if (result.caught is not null) throw result.caught;
                after = selector();
                return after!.CompareTo(before!) > 0;
            },
            () => $"Expected {selectorExpr ?? "<selector>"} to increase, but went from {ValueFormatter.Format(before)} to {ValueFormatter.Format(after)}",
            isSkipped);
        }

        public static CheckOperation Decrements<TState>(
            Func<Task> func, Func<TState> selector, string? selectorExpr,
            SlotContainer slots, bool isSkipped)
            where TState : IComparable<TState>
        {
            TState? before = default;
            TState? after = default;

            return CheckOperation.Async(async () =>
            {
                before = selector();
                var result = await FuncExecution.EnsureAsync(func, slots).ConfigureAwait(false);
                if (result.caught is not null) throw result.caught;
                after = selector();
                return after!.CompareTo(before!) < 0;
            },
            () => $"Expected {selectorExpr ?? "<selector>"} to decrease, but went from {ValueFormatter.Format(before)} to {ValueFormatter.Format(after)}",
            isSkipped);
        }

        private static string BuildMessage<TState>(
            TState? before, TState? after, TState? from, TState? to,
            bool hasFrom, bool hasTo, string? selExpr, string? fromExpr, string? toExpr)
        {
            var sel = selExpr ?? "<selector>";

            if (hasFrom && !EqualityComparer<TState>.Default.Equals(before!, from!))
                return $"Expected {sel} to start at {ExprFormat.Inline(from, fromExpr)}, but was {ValueFormatter.Format(before)}";

            if (hasTo && !EqualityComparer<TState>.Default.Equals(after!, to!))
                return $"Expected {sel} to end at {ExprFormat.Inline(to, toExpr)}, but was {ValueFormatter.Format(after)}";

            return $"Expected {sel} to change, but remained {ValueFormatter.Format(before)}";
        }
    }
}
