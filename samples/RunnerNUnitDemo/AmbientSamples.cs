using Catchy;
using Catchy.NUnit;
using NUnit.Framework;
using static Catchy.AmbientAlias;
using static Catchy.AmbientSoft;

namespace RunnerNUnitDemo;

/// <summary>
/// Real-world NUnit examples with Catchy assertions.
/// Demonstrates practical validation with automatic cleanup.
/// </summary>
public sealed class AmbientSamples : AmbientNUnitBase
{
    [Test]
    public async Task Customer_data_comprehensive_validation()
    {
        var customer = new Customer
        {
            Id = 123,
            Name = "John Doe",
            Email = "john@example.com",
            IsActive = true
        };

        await Check.That(customer.Id).IsGreaterThan(0);
        await Check.That(customer.Name).IsNotEmpty();
        await Check.That(customer.Email).Contains("@");
        await Check.That(customer.IsActive).IsTrue();
    }

    [Test]
    public async Task Invoice_validation_with_soft_assertions_for_all_fields()
    {
        var invoice = new Invoice
        {
            Number = "INV-001",
            Amount = 1500.00m,
            Items = ["Service A", "Service B", "Service C"],
            IsPaid = false
        };

        // Check all fields comprehensively
        await Verify.That(invoice.Number).StartsWith("INV-");
        await Verify.That(invoice.Amount).IsGreaterThan(0);
        await Verify.That(invoice.Items).HasCount(3);
        await Verify.That(invoice.IsPaid).IsFalse();
    }

    [Test]
    public async Task HTTP_request_structure_validation()
    {
        var request = new HttpRequest
        {
            Method = "POST",
            Url = "https://api.example.com/users",
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer token123",
                ["Content-Type"] = "application/json"
            }
        };

        await Check.That(request.Method).Is("POST");
        await Check.That(request.Url).Contains("https://");
        await Check.That(request.Headers).HasCount(2);
        await Check.That(request.Headers["Content-Type"]).Is("application/json");
    }

    [Test]
    public async Task Using_Check_alias_for_cleaner_API()
    {
        var numbers = new List<int> { 10, 20, 30, 40, 50 };

        await Stateless.Assert.That(numbers).HasCount(5);
        await Stateless.Assert.That(numbers[0]).Is(10);
        await Stateless.Assert.That(numbers).Contains(30);
    }

    [Test]
    public async Task Manual_flush_with_custom_soft_asserter()
    {
        var customSoft = Asserter.NewSoft();

        await customSoft.That(5).Is(5);
        await customSoft.That("test").IsNotEmpty();

        // Assertion-style manual flush
        await Stateless.Assert.That(customSoft.SoftState).HasNoErrors();
    }
}

// Test models
record Customer { public int Id { get; init; } public string Name { get; init; } = ""; public string Email { get; init; } = ""; public bool IsActive { get; init; } }
record Invoice { public string Number { get; init; } = ""; public decimal Amount { get; init; } public string[] Items { get; init; } = Array.Empty<string>(); public bool IsPaid { get; init; } }
record HttpRequest { public string Method { get; init; } = ""; public string Url { get; init; } = ""; public Dictionary<string, string> Headers { get; init; } = new(); }
