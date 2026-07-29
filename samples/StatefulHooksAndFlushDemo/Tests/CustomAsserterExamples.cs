using Catchy;
using static Catchy.StatelessAlias;

namespace StatefulHooksAndFlushDemo.Tests;

/// <summary>
/// Real-world examples showing custom asserter configuration with hooks and manual flush.
/// Demonstrates practical scenarios where you need fine-grained control over assertion lifecycle.
/// </summary>
public class CustomAsserterExamples
{
    [Test]
    public async Task Logging_hook_captures_all_assertion_activity()
    {
        var log = new List<string>();

        var asserter = Asserter.NewStateful(cfg =>
            cfg.OnAssertion = [info =>
            {
                log.Add($"[{info.Status}] {string.Join(" -> ", info.Links)}");
                return ValueTask.CompletedTask;
            }]);

        var user = new { Name = "Alice", Age = 30 };

        await asserter.That(user.Name).IsNotEmpty();
        await asserter.That(user.Age).IsGreaterThan(18);

        // Verify logging worked
        await Check.That(log).HasCount(2);
        await Check.That(log[0]).Contains("[Passed]");
        await Check.That(log[1]).Contains("[Passed]");
    }

    [Test]
    public async Task Performance_monitoring_hook_tracks_execution_time()
    {
        var timings = new List<TimeSpan>();

        var asserter = Asserter.NewStateful(cfg =>
            cfg.OnExecution = [async (pipeline, next) =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await next();
                timings.Add(sw.Elapsed);
            }]);

        await asserter.That("test").IsNotEmpty();
        await asserter.That(42).Is(42);

        // Verify we captured timings
        await Check.That(timings).HasCount(2);
        await Check.That(timings[0]).IsGreaterThan(TimeSpan.Zero);
    }

    [Test]
    public async Task Soft_assertion_with_custom_failure_tracking()
    {
        var failures = new List<string>();

        var verify = Asserter.NewSoft(cfg =>
            cfg.OnSoftFailure = [info =>
            {
                failures.Add($"Failed: {info.Exception?.Message}");
                return ValueTask.CompletedTask;
            }]);

        var order = new { Id = 0, Total = -10m, Status = "" };

        // Collect multiple failures
        await verify.That(order.Id).IsGreaterThan(0);
        await verify.That(order.Total).IsGreaterThan(0);
        await verify.That(order.Status).IsNotEmpty();

        // Verify we tracked all failures
        await Check.That(failures).HasCount(3);
        await Check.That(verify.SoftState.ErrorCount).Is(3);
    }

    [Test]
    public async Task Manual_flush_with_HasNoErrors_assertion()
    {
        var verify = Asserter.NewSoft();

        var product = new { Name = "Widget", Price = 9.99m };

        await verify.That(product.Name).IsNotEmpty();
        await verify.That(product.Price).IsGreaterThan(0);

        // Manual flush - assertion style
        await Check.That(verify.SoftState).HasNoErrors();
    }

    [Test]
    public async Task Manual_flush_detects_failures()
    {
        var verify = Asserter.NewSoft();

        await verify.That(5).Is(10); // Will fail

        // This will throw because there are errors
        try
        {
            await Check.That(verify.SoftState).HasNoErrors();
            throw new InvalidOperationException("Should have thrown!");
        }
        catch (Exception ex) when (ex.Message.Contains("softAssert assertion"))
        {
            // Expected - softAssert state has errors
        }
    }

    [Test]
    public async Task Checkpoint_and_revert_for_staged_validation()
    {
        var softAssert = Asserter.NewSoft();

        // First stage - validate basics
        await softAssert.That(1).Is(2); // Fails
        var checkpoint = softAssert.Checkpoint();

        // Second stage - add more checks
        await softAssert.That(3).Is(4); // Also fails

        // Revert to checkpoint - only first failure remains
        softAssert.Revert(checkpoint);

        await Check.That(softAssert).Errors().HasCount(1);
    }

    [Test]
    public async Task OnFlush_hook_for_custom_error_reporting()
    {
        var softAssert = Asserter.NewSoft();
        var errorReport = "";

        softAssert.SoftState.OnFlush = [aggregate =>
        {
            errorReport = $"Total failures: {aggregate.InnerExceptions.Count}";
            return Task.CompletedTask;
        }];

        await softAssert.That(1).Is(2);
        await softAssert.That(3).Is(4);

        try
        {
            await softAssert.SoftState.FlushIfNeeded();
        }
        catch
        {
            // Expected
        }

        await Check.That(errorReport).Contains("Total failures: 2");
    }

    [Test]
    public async Task Multiple_custom_asserters_with_different_configs()
    {
        var loggedAssert = Asserter.NewStateful(cfg =>
            cfg.OnAssertion = [info =>
            {
                Console.WriteLine($"Logged: {info.Status}");
                return ValueTask.CompletedTask;
            }]);

        var softAssert = Asserter.NewSoft();

        // Use different asserters for different purposes
        await loggedAssert.That("production").IsNotEmpty(); // Logged
        await softAssert.That(0).IsGreaterThan(0); // Accumulated

        await Check.That(softAssert).Errors().HasCount(1);
    }
}
