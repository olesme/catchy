using Catchy;
using Catchy.Sdk;

namespace ExtensibilityMatrixDemo;

public static class CustomExtensions
{
    public static ValueAssertions<Invoice?> HasNonEmptyNumber(this ValueAssertions<Invoice?> a)
    {
        a.Link("HasNonEmptyNumber");
        a.Op(v => CheckOperation.Sync(
            () => !string.IsNullOrWhiteSpace(v.GetValue()?.Number),
            () => "Expected invoice number to be non-empty",
            v.IsSkipped()));
        return a;
    }
}

public static partial class NumericCustomExtensions
{
    [GenerateTypedOverloads(typeof(long), typeof(short), typeof(byte))]
    public static ValueAssertions<int> IsEvenCustom(this ValueAssertions<int> a)
    {
        a.Link("IsEvenCustom");
        a.Op(v => CheckOperation.Sync(
            () => v.GetValue() % 2 == 0,
            () => $"Expected {v.GetValue()} to be even",
            v.IsSkipped()));
        return a;
    }
}
