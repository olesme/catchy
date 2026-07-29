using Catchy;

namespace ExtensibilityMatrixDemo;

public sealed class Invoice
{
    public string Number { get; init; } = string.Empty;
    public decimal Total { get; init; }
}

[Assertable]
public sealed class Customer
{
    [AssertMember]
    public string Name { get; init; } = string.Empty;

    [AssertMember]
    public int Age { get; init; }
}

[AssertFor(typeof(Invoice))]
public static partial class InvoiceAssertions
{
    [Assertion("have positive total")]
    public static bool HasPositiveTotal(Invoice invoice) => invoice.Total > 0;

    [Assertion("have expected number")]
    public static bool HasNumber(Invoice invoice, string expected)
        => string.Equals(invoice.Number, expected, StringComparison.Ordinal);
}
