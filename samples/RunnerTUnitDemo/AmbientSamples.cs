using Catchy;
using static Catchy.AmbientAlias;

namespace RunnerTUnitDemo;

/// <summary>
/// Real-world TUnit examples with Catchy assertions.
/// Uses Check alias to avoid conflict with TUnit.Assertions.Assert.
/// </summary>
public sealed class AmbientSamples
{
    [Test]
    public async Task Employee_record_validation()
    {
        var employee = new Employee
        {
            Id = 1001,
            Name = "Bob Johnson",
            Department = "Engineering",
            Salary = 75000m
        };

        // Using Check alias (cleaner API, no conflicts)
        await Check.That(employee.Id).IsGreaterThan(0);
        await Check.That(employee.Name).IsNotEmpty();
        await Check.That(employee.Department).Is("Engineering");
        await Check.That(employee.Salary).IsGreaterThan(0);
    }

    [Test]
    public async Task Payment_transaction_comprehensive_validation_with_soft()
    {
        var transaction = new Transaction
        {
            TransactionId = "TXN-999",
            Amount = 250.00m,
            Currency = "USD",
            Status = "Completed",
            Timestamp = DateTime.UtcNow
        };

        // Soft assertions collect all failures
        await Check.Soft.That(transaction.TransactionId).StartsWith("TXN-");
        await Check.Soft.That(transaction.Amount).IsGreaterThan(0);
        await Check.Soft.That(transaction.Currency).Is("USD");
        await Check.Soft.That(transaction.Status).Is("Completed");
        await Check.Soft.That(transaction.Timestamp).IsLessThanOrEqualTo(DateTime.UtcNow);
    }

    [Test]
    public async Task Web_request_structure_check()
    {
        var request = new WebRequest
        {
            Endpoint = "/api/users",
            Method = "GET",
            QueryParams = new Dictionary<string, string>
            {
                ["page"] = "1",
                ["limit"] = "10"
            }
        };

        await Check.That(request.Endpoint).StartsWith("/api/");
        await Check.That(request.Method).Is("GET");
        await Check.That(request.QueryParams).HasCount(2);
        await Check.That(request.QueryParams["limit"]).Is("10");
    }

    [Test]
    public async Task Manual_soft_flush_with_assertion_style()
    {
        var customSoft = Asserter.NewSoft();

        await customSoft.That("test").IsNotEmpty();
        await customSoft.That(42).IsGreaterThan(0);

        // Assertion-style flush
        await Stateless.Assert.That(customSoft.SoftState).HasNoErrors();
    }

    [Test]
    public async Task Health_status_comprehensive_check_with_soft()
    {
        var health = new HealthStatus
        {
            ServiceName = "PaymentService",
            IsHealthy = true,
            Uptime = TimeSpan.FromHours(120),
            ActiveConnections = 42
        };

        await Check.Soft.That(health.ServiceName).IsNotEmpty();
        await Check.Soft.That(health.IsHealthy).IsTrue();
        await Check.Soft.That(health.Uptime).IsGreaterThan(TimeSpan.Zero);
        await Check.Soft.That(health.ActiveConnections).IsGreaterThan(0);
    }
}

// Test domain models
record Employee { public int Id { get; init; } public string Name { get; init; } = ""; public string Department { get; init; } = ""; public decimal Salary { get; init; } }
record Transaction { public string TransactionId { get; init; } = ""; public decimal Amount { get; init; } public string Currency { get; init; } = ""; public string Status { get; init; } = ""; public DateTime Timestamp { get; init; } }
record WebRequest { public string Endpoint { get; init; } = ""; public string Method { get; init; } = ""; public Dictionary<string, string> QueryParams { get; init; } = new(); }
record HealthStatus { public string ServiceName { get; init; } = ""; public bool IsHealthy { get; init; } public TimeSpan Uptime { get; init; } public int ActiveConnections { get; init; } }
