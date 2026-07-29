using Catchy;
using Catchy.MSTest;
using static Catchy.AmbientAlias;
using static Catchy.AmbientSoft;

namespace RunnerMSTestDemo;

/// <summary>
/// Real-world MSTest examples with Catchy assertions.
/// Uses Check alias to avoid conflict with MSTest.Assert.
/// </summary>
[TestClass]
public sealed class AmbientSamples : AmbientMSTestBase
{
    [TestMethod]
    public async Task Product_inventory_comprehensive_check()
    {
        var product = new Product
        {
            Sku = "PROD-001",
            Name = "Wireless Mouse",
            Stock = 50,
            Price = 29.99m
        };

        // Using Check alias (no conflict with MSTest.Assert)
        await Check.That(product.Sku).StartsWith("PROD-");
        await Check.That(product.Name).IsNotEmpty();
        await Check.That(product.Stock).IsGreaterThan(0);
        await Check.That(product.Price).IsGreaterThan(0);
    }

    [TestMethod]
    public async Task Order_summary_with_soft_assertions()
    {
        var order = new OrderSummary
        {
            OrderId = "ORD-12345",
            CustomerName = "Jane Smith",
            ItemCount = 3,
            Total = 149.97m,
            Status = "Shipped"
        };

        // Soft assertions accumulate all failures
        await Verify.That(order.OrderId).StartsWith("ORD-");
        await Verify.That(order.CustomerName).IsNotEmpty();
        await Verify.That(order.ItemCount).IsGreaterThan(0);
        await Verify.That(order.Total).IsGreaterThan(0);
        await Verify.That(order.Status).Is("Shipped");
    }

    [TestMethod]
    public async Task API_response_validation()
    {
        var response = new ApiResponse
        {
            Success = true,
            Message = "Operation completed successfully",
            Data = new Dictionary<string, object>
            {
                ["userId"] = 123,
                ["username"] = "alice"
            }
        };

        await Check.That(response.Success).IsTrue();
        await Check.That(response.Message).Contains("success");
        await Check.That(response.Data).HasCount(2);
        await Check.That((int)response.Data["userId"]).Is(123);
    }

    [TestMethod]
    public async Task Manual_flush_with_custom_soft()
    {
        var localAsserter = Asserter.NewSoft();

        await localAsserter.That("demo").IsNotEmpty();
        await localAsserter.That(100).Is(100);

        // Assertion-style flush
        await Stateless.Assert.That(localAsserter).HasNoErrors();
    }

    [TestMethod]
    public async Task Settings_validation_using_soft_for_comprehensive_report()
    {
        var settings = new Settings
        {
            DatabaseUrl = "https://db.example.com",
            Timeout = 30,
            EnableLogging = true,
            AllowedHosts = ["localhost", "example.com"]
        };

        await Verify.That(settings.DatabaseUrl).StartsWith("https://");
        await Verify.That(settings.Timeout).IsGreaterThan(0);
        await Verify.That(settings.EnableLogging).IsTrue();
        await Verify.That(settings.AllowedHosts).HasCount(2);
    }
}

// Test domain models
record Product { public string Sku { get; init; } = ""; public string Name { get; init; } = ""; public int Stock { get; init; } public decimal Price { get; init; } }
record OrderSummary { public string OrderId { get; init; } = ""; public string CustomerName { get; init; } = ""; public int ItemCount { get; init; } public decimal Total { get; init; } public string Status { get; init; } = ""; }
record ApiResponse { public bool Success { get; init; } public string Message { get; init; } = ""; public Dictionary<string, object> Data { get; init; } = new(); }
record Settings { public string DatabaseUrl { get; init; } = ""; public int Timeout { get; init; } public bool EnableLogging { get; init; } public string[] AllowedHosts { get; init; } = Array.Empty<string>(); }
