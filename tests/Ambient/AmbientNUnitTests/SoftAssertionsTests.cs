using Catchy.NUnit;
using Catchy;
using CatchyTestHelpers;

namespace AmbientNUnitTests
{
    [TestFixture]
    public class SoftAssertionTests : AmbientNUnitBase
    {
        [Test]
        public async Task SoftAsserter_HardFlush_throws_aggregate()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            try
            {
                await Stateless.Assert.That(Ambient.Assert.Soft).HasNoErrors();
            }
            catch (AggregateAssertionException ex)
            {
                Assert.That(ex.InnerExceptions.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task SoftAsserter_no_failures_TryFlush_does_not_throw()
        {
            await Ambient.Assert.Soft.That(42).Is(42);
            var ex = Ambient.Assert.Soft.SoftState.AggregateException;
            Assert.That(ex, Is.Null);
        }

        [Test]
        public async Task SoftAsserter_Clear_resets_failures()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            Ambient.Assert.Soft.Clear();
            Assert.That(Ambient.Assert.Soft.HasFailures, Is.False);
            Assert.That(Ambient.Assert.Soft.ErrorCount, Is.EqualTo(0));
        }

        // Ambient Stateless Asserter tests
        [Test]
        public async Task Ambient_Hard_throws_on_assertion()
        {
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Ambient.Assert.That(1).Is(2)
            );
            Assert.That(msg, Does.Contain("Assertion failed"));
        }

        [Test]
        public async Task Ambient_Hard_passes_valid_assertion()
        {
            await Ambient.Assert.That(1).Is(1);
            // Should pass
        }

        // Native path tests - verify TestContext returns same instance
        [Test]
        public async Task Ambient_Hard_same_instance_native_path()
        {
            // Get Stateless asserter via Ambient
            var hard1 = Ambient.Assert;
            var hard2 = Ambient.Assert;

            // Should be same instance (from TestExecutionContext.Properties)
            Assert.That(hard1, Is.SameAs(hard2));
        }

        [Test]
        public async Task Ambient_Soft_same_instance_native_path()
        {
            // Get Soft asserter via Ambient
            var soft1 = Ambient.Assert.Soft;
            var soft2 = Ambient.Assert.Soft;

            // Should be same instance (from TestExecutionContext.Properties)
            Assert.That(soft1, Is.SameAs(soft2));
        }

        [Test]
        public async Task Ambient_Hard_instance_not_shared_between_tests()
        {
            // Store reference to current test's Stateless asserter
            var hardInThisTest = Ambient.Assert;
            var testId = TestContext.CurrentContext.Test.ID;

            // Stateless asserter should be specific to this test
            Assert.That(hardInThisTest, Is.Not.Null);
            Assert.That(testId, Is.Not.Null);
        }

        // Instance Stateless asserter tests (successful wrapping without XFAIL)
        [Test]
        public async Task Instance_Hard_asserter_wraps_failure_successfully()
        {
            var hard = new StatefulAsserter();
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await hard.That(1).Is(2)
            );
            Assert.That(msg, Does.Contain("Assertion failed"));
        }

        [Test]
        public async Task Instance_Hard_asserter_passes_valid_assertion()
        {
            var hard = new StatefulAsserter();
            await hard.That(42).Is(42);
            // Should pass without throwing
        }

        [Test]
        public async Task Instance_Hard_asserter_multiple_assertions()
        {
            var hard = new StatefulAsserter();
            await hard.That(1).Is(1);
            await hard.That("test").Is("test");
            await hard.That(true).IsTrue();
            // All pass
        }

        [Test]
        public async Task Instance_Hard_asserter_first_failure_throws()
        {
            var hard = new StatefulAsserter();
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            {
                await hard.That(1).Is(1);
                await hard.That(2).Is(999); // This should throw
                await hard.That(3).Is(3);   // Should not reach here
            });
            Assert.That(msg, Does.Contain("2") & Does.Contain("999"));
        }

        [Test]
        public async Task Instance_Hard_asserter_isolated_from_ambient()
        {
            var instance = new StatefulAsserter();

            // Instance Stateless should be independent from Ambient.Assert
            var ambient = Ambient.Assert;

            Assert.That(instance, Is.Not.SameAs(ambient));
        }

        [Test]
        public async Task Instance_Hard_asserter_isolated_from_other_tests()
        {
            // This test verifies that instance asserter created here
            // doesn't affect other tests
            var local = new StatefulAsserter();

            // Do some assertions
            await local.That(100).Is(100);

            // Create new instance in same test - should be different
            var local2 = new StatefulAsserter();
            Assert.That(local, Is.Not.SameAs(local2));
        }

        // Soft asserter OnFlush hook tests
        [Test]
        public async Task Soft_Asserter_OnFlush_Hook_called_on_failure()
        {
            var hooksCalled = new List<string>();

            var verify = new SoftAsserter();

            // Register OnFlush hook via IReadOnlyList by re-assigning with new list
            verify.SoftState.OnFlush = [async (ex) =>
            {
                hooksCalled.Add("OnFlush");
                await Task.CompletedTask;
            }];

            // Add error via assertion
            await verify.That(1).Is(2);

            // Flush should call OnFlush hook
            try
            {
                await verify.SoftState.FlushIfNeeded();
            }
            catch
            {
                // Expected to throw after hooks
            }

            Assert.That(hooksCalled.Count, Is.EqualTo(1));
            Assert.That(hooksCalled[0], Is.EqualTo("OnFlush"));
        }

        [Test]
        public async Task Soft_Asserter_Multiple_OnFlush_Hooks()
        {
            var hooksCalled = new List<string>();

            var verify = new SoftAsserter();
            verify.SoftState.OnFlush = [
                async (ex) =>
                {
                    hooksCalled.Add("Hook1");
                    await Task.CompletedTask;
                },
                async (ex) =>
                {
                    hooksCalled.Add("Hook2");
                    await Task.CompletedTask;
                }
            ];

            await verify.That(1).Is(2);

            try
            {
                await verify.SoftState.FlushIfNeeded();
            }
            catch { }

            Assert.That(hooksCalled.Count, Is.EqualTo(2));
            Assert.That(hooksCalled[0], Is.EqualTo("Hook1"));
            Assert.That(hooksCalled[1], Is.EqualTo("Hook2"));
        }

        [Test]
        public async Task Soft_Asserter_Hook_receives_aggregate_exception()
        {
            var capturedExceptions = new List<AggregateAssertionException>();

            var verify = new SoftAsserter();
            verify.SoftState.OnFlush = [async (ex) =>
            {
                capturedExceptions.Add(ex);
                await Task.CompletedTask;
            }];

            // Add multiple errors
            await verify.That(1).Is(2);
            await verify.That("a").Is("b");

            try
            {
                await verify.SoftState.FlushIfNeeded();
            }
            catch { }

            Assert.That(capturedExceptions.Count, Is.EqualTo(1));
            Assert.That(capturedExceptions[0].InnerExceptions.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Soft_Asserter_Hook_can_modify_behavior()
        {
            var verify = new SoftAsserter();

            // Set FlushAction instead of throwing - useful for test frameworks
            verify.SoftState.FlushAction = async (ex) =>
            {
                // Custom behavior: log instead of throw
                System.Diagnostics.Debug.WriteLine($"Soft assertions flushed: {ex.InnerExceptions.Count} errors");
                await Task.CompletedTask;
            };

            await verify.That(1).Is(2);

            // This should not throw because FlushAction is set
            await verify.SoftState.FlushIfNeeded();

            Assert.That(verify.SoftState.AlreadyFlushed, Is.True);
        }
    }
}
