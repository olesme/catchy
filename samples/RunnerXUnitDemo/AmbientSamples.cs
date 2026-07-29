using Catchy;
using Catchy.XUnit;
using Xunit;
using static Catchy.AmbientAlias;

namespace RunnerXUnitDemo;

/// <summary>
/// Real-world xUnit test examples demonstrating Catchy fluent assertions.
/// Shows practical validation scenarios with automatic ambient lifecycle.
/// </summary>
public sealed class AmbientSamples : CatchyTestBase
{
    [Fact]
    public async Task User_registration_validates_all_required_fields()
    {
        var user = new User { Name = "Alice", Email = "alice@example.com", Age = 25 };

        await Check.That(user.Name).IsNotEmpty();
        await Check.That(user.Email).Contains("@");
        await Check.That(user.Age).IsGreaterThan(18);
    }

    [Fact]
    public async Task Order_validation_with_soft_assertions_checks_all_fields()
    {
        var order = new Order
        {
            Items = ["Book", "Pen"],
            Total = 25.50m,
            Status = "Pending"
        };

        // Validate multiple aspects - all failures reported together
        await Check.Soft.That(order.Items).HasCount(2);
        await Check.Soft.That(order.Total).IsGreaterThan(0);
        await Check.Soft.That(order.Status).Is("Pending");

        // Auto-flushed by base class teardown
    }

    [Fact]
    public async Task API_response_comprehensive_validation()
    {
        var response = new ApiResponse
        {
            StatusCode = 200,
            Body = "{\"success\": true}",
            Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
        };

        await Check.That(response.StatusCode).Is(200);
        await Check.That(response.Body).Contains("success");
        await Check.That(response.Headers["Content-Type"]).Contains("json");
    }

    [Fact]
    public async Task Product_validation_with_comprehensive_soft_checks()
    {
        var product = new Product
        {
            Name = "Laptop",
            Price = 999.99m,
            InStock = true,
            Tags = ["electronics", "computers"]
        };

        // Check all properties even if some fail
        await Check.Soft.That(product.Name).IsNotEmpty();
        await Check.Soft.That(product.Price).IsGreaterThan(0);
        await Check.Soft.That(product.InStock).IsTrue();
        await Check.Soft.That(product.Tags).HasCount(2);
    }

    [Fact]
    public async Task Manual_soft_flush_example_with_assertion_style()
    {
        var customSoftAssert = Asserter.NewSoft();

        await customSoftAssert.That("test").IsNotEmpty();
        await customSoftAssert.That(42).IsGreaterThan(0);

        // Manual assertion-style flush
        await Stateless.Assert.That(customSoftAssert.SoftState).HasNoErrors();
    }

    [Fact]
    public async Task Using_Stateless_for_validation()
    {
        var items = new[] { 1, 2, 3, 4, 5 };

        await Stateless.Assert.That(items).HasCount(5);
        await Stateless.Assert.That(items).Contains(3);
        await Stateless.Assert.That(items[0]).Is(1);
    }
}

// Test domain models
record User { public string Name { get; init; } = ""; public string Email { get; init; } = ""; public int Age { get; init; } }
record Order { public string[] Items { get; init; } = Array.Empty<string>(); public decimal Total { get; init; } public string Status { get; init; } = ""; }
record ApiResponse { public int StatusCode { get; init; } public string Body { get; init; } = ""; public Dictionary<string, string> Headers { get; init; } = new(); }
record Product { public string Name { get; init; } = ""; public decimal Price { get; init; } public bool InStock { get; init; } public string[] Tags { get; init; } = Array.Empty<string>(); }
