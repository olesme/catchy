namespace Catchy.Sdk
{
    public static class NullChecks
    {
        public static CheckOperation IsNull(object? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null,
                () => $"Expected null, but was {ValueFormatter.Format(actual)}",
                isSkipped);

        public static CheckOperation IsNotNull(object? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null,
                () => "Expected a value, but was null",
                isSkipped);

        public static CheckOperation IsDefault<T>(T? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => EqualityComparer<T?>.Default.Equals(actual, default),
                () => $"Expected default value, but was {ValueFormatter.Format(actual)}",
                isSkipped);
    }
}
