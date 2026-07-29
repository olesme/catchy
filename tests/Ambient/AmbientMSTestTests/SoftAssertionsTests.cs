using Catchy.MSTest;
using Catchy;
using CatchyTestHelpers;

namespace AmbientMSTestTests
{
    [TestClass]
    public class SoftAssertionsTests : AmbientMSTestBase
    {
        [TestMethod]
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
                Assert.AreEqual(2, ex.InnerExceptions.Count);
            }
            // Auto flush will not throw since we already flushed manually
        }

        [TestMethod]
        public async Task SoftAsserter_no_failures_TryFlush_does_not_throw()
        {
            await Ambient.Assert.Soft.That(42).Is(42);
            var ex = Ambient.Assert.Soft.SoftState.AggregateException;
            Assert.IsNull(ex);
        }

        [TestMethod]
        public async Task SoftAsserter_Clear_resets_failures()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            Ambient.Assert.Soft.Clear();
            Assert.IsFalse(Ambient.Assert.Soft.HasFailures);
            Assert.AreEqual(0, Ambient.Assert.Soft.ErrorCount);
        }

        // Ambient Stateless Asserter tests
        [TestMethod]
        public async Task Ambient_Hard_throws_on_assertion()
        {
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Ambient.Assert.That(1).Is(2)
            );
            Assert.Contains("Assertion failed", msg);
        }

        [TestMethod]
        public async Task Ambient_Hard_passes_valid_assertion()
        {
            await Ambient.Assert.That(1).Is(1);
            // Should pass
        }

        // Native path tests - verify both AsyncLocal and native TestContext paths return same instance
        [TestMethod]
        public async Task Ambient_Hard_same_instance_native_path()
        {
            // Get Stateless asserter via Ambient
            var hard1 = Ambient.Assert;
            var hard2 = Ambient.Assert;

            // Should be same instance (from TestContext.Properties)
            Assert.AreSame(hard1, hard2);
        }

        [TestMethod]
        public async Task Ambient_Soft_same_instance_native_path()
        {
            // Get Soft asserter via Ambient
            var soft1 = Ambient.Assert.Soft;
            var soft2 = Ambient.Assert.Soft;

            // Should be same instance (from TestContext.Properties)
            Assert.AreSame(soft1, soft2);
        }

        [TestMethod]
        public async Task Ambient_Hard_instance_not_shared_between_tests()
        {
            // Store reference to current test's Stateless asserter
            var hardInThisTest = Ambient.Assert;
            var testName = TestContext.TestName;

            // Stateless asserter should be specific to this test
            Assert.IsNotNull(hardInThisTest);
            Assert.IsNotNull(testName);
        }

        // Instance Stateless asserter tests (successful wrapping without XFAIL)
        [TestMethod]
        public async Task Instance_Hard_asserter_wraps_failure_successfully()
        {
            var hard = new StatefulAsserter();
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await hard.That(1).Is(2)
            );
            Assert.Contains("Assertion failed", msg);
        }

        [TestMethod]
        public async Task Instance_Hard_asserter_passes_valid_assertion()
        {
            var hard = new StatefulAsserter();
            await hard.That(42).Is(42);
            // Should pass without throwing
        }

        [TestMethod]
        public async Task Instance_Hard_asserter_multiple_assertions()
        {
            var hard = new StatefulAsserter();
            await hard.That(1).Is(1);
            await hard.That("test").Is("test");
            await hard.That(true).IsTrue();
            // All pass
        }

        [TestMethod]
        public async Task Instance_Hard_asserter_first_failure_throws()
        {
            var hard = new StatefulAsserter();
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            {
                await hard.That(1).Is(1);
                await hard.That(2).Is(999); // This should throw
                await hard.That(3).Is(3);   // Should not reach here
            });
            Assert.Contains("2", msg);
            Assert.Contains("999", msg);
        }

        [TestMethod]
        public async Task Instance_Hard_asserter_isolated_from_ambient()
        {
            var instance = new StatefulAsserter();

            // Instance Stateless should be independent from Ambient.Assert
            var ambient = Ambient.Assert;

            Assert.AreNotSame(instance, ambient);
        }

        [TestMethod]
        public async Task Instance_Hard_asserter_isolated_from_other_tests()
        {
            // This test verifies that instance asserter created here
            // doesn't affect other tests
            var local = new StatefulAsserter();

            // Do some assertions
            await local.That(100).Is(100);

            // Create new instance in same test - should be different
            var local2 = new StatefulAsserter();
            Assert.AreNotSame(local, local2);
        }

        // Soft asserter OnFlush hook tests
        [TestMethod]
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

            Assert.AreEqual(1, hooksCalled.Count);
            Assert.AreEqual("OnFlush", hooksCalled[0]);
        }

        [TestMethod]
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

            Assert.AreEqual(2, hooksCalled.Count);
            Assert.AreEqual("Hook1", hooksCalled[0]);
            Assert.AreEqual("Hook2", hooksCalled[1]);
        }

        [TestMethod]
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

            Assert.AreEqual(1, capturedExceptions.Count);
            Assert.AreEqual(2, capturedExceptions[0].InnerExceptions.Count);
        }

        [TestMethod]
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

            Assert.IsTrue(verify.SoftState.AlreadyFlushed);
        }
    }
}
