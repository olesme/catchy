namespace Catchy.Sdk
{
    public sealed class CheckOperation
    {
        public Func<bool>? PassesSync { get; }
        public Func<Task<bool>>? PassesAsync { get; }
        public Func<string> FailBody { get; }
        public bool IsSkipped { get; }
        public bool IsOr { get; }
        public object? ActualObject { get; }
        public Func<(string label, object? value, string? expr)[]>? HintsFactory { get; }
        /// <summary>
        /// Optional. When non-null and returns a non-empty list, ThrowAsync wraps the
        /// result in <see cref="AggregateAssertionException"/> instead of plain
        /// <see cref="AssertionException"/>, making inner failures enumerable on catch.
        /// </summary>
        public Func<IReadOnlyList<AssertionException>?>? InnerExceptionsFactory { get; }
        public bool IsAsync => PassesAsync is not null;

        public string FailBecause() => FailBody();

        private CheckOperation(Func<bool> passes, Func<string> failBody, bool isSkipped, bool isOr,
            object? actualObject, Func<(string, object?, string?)[]>? hintsFactory,
            Func<IReadOnlyList<AssertionException>?>? innerExceptionsFactory)
        {
            PassesSync = passes; FailBody = failBody;
            IsSkipped = isSkipped; IsOr = isOr; ActualObject = actualObject;
            HintsFactory = hintsFactory; InnerExceptionsFactory = innerExceptionsFactory;
        }

        private CheckOperation(Func<Task<bool>> passesAsync, Func<string> failBody, bool isSkipped, bool isOr,
            object? actualObject, Func<(string, object?, string?)[]>? hintsFactory,
            Func<IReadOnlyList<AssertionException>?>? innerExceptionsFactory)
        {
            PassesAsync = passesAsync; FailBody = failBody;
            IsSkipped = isSkipped; IsOr = isOr; ActualObject = actualObject;
            HintsFactory = hintsFactory; InnerExceptionsFactory = innerExceptionsFactory;
        }

        public static CheckOperation Sync(Func<bool> passes, Func<string> failBody,
            bool isSkipped = false, object? actualObject = null,
            Func<(string, object?, string?)[]>? hintsFactory = null,
            Func<IReadOnlyList<AssertionException>?>? innerExceptionsFactory = null)
            => new(passes, failBody, isSkipped, false, actualObject, hintsFactory, innerExceptionsFactory);

        public static CheckOperation Async(Func<Task<bool>> passesAsync, Func<string> failBody,
            bool isSkipped = false, object? actualObject = null,
            Func<(string, object?, string?)[]>? hintsFactory = null,
            Func<IReadOnlyList<AssertionException>?>? innerExceptionsFactory = null)
            => new(passesAsync, failBody, isSkipped, false, actualObject, hintsFactory, innerExceptionsFactory);
    }
}
