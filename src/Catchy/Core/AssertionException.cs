namespace Catchy
{
    public class AssertionException(string message, Exception? inner = null) : Exception(message, inner)
    {
        public string? Chain { get; init; }
        public string? Body { get; init; }
        public SourceLocation? AssertionSource { get; init; }
    }

    public class AggregateAssertionException(IReadOnlyList<AssertionException> innerExceptions, string message)
        : AssertionException(message)
    {
        public IReadOnlyList<AssertionException> InnerExceptions { get; } = innerExceptions;
    }
}
