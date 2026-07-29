using Catchy.Sdk;
using Catchy.Configuration;
using Catchy;

namespace AmbientReqnrollTests.StepDefinitions
{
    [Binding]
    public class StatefulAssertionsFeatureStepDefinitions(StatefulAsserter assert, ScenarioContext ctx)
    {
        [Given("asserter source is {word}")]
        public void GivenEvidanceEntityIsWordEvidance(AsserterSourceMode mode)
        {
            ctx[AsserterSourceModeKey] = mode;
        }

        [When("I got soft fail")]
        public async Task WhenIGotSoftFail()
        {
            Thread.Sleep(100);
            await GetAsserter().Soft.That(true).IsFalse();
            Thread.Sleep(100);
        }

        [Then("the soft fails count should be {int}")]
        public async Task ThenTheSoftFailsCountShouldBe(int count)
        {
            Thread.Sleep(100);
            await Stateless.Assert.That(GetAsserter().Soft).Errors().HasCount(count);
            Thread.Sleep(100);
        }

        [When("I cleanup soft fails")]
        public void WhenICleanupSoftFails()
        {
            Thread.Sleep(100);
            GetAsserter().Soft.Clear();
            Thread.Sleep(100);
        }

        [When("I flush hard")]
        public async Task WhenIFlushHard()
        {
            Thread.Sleep(100);
            await Stateless.Assert.That(GetAsserter().Soft).HasNoErrors();
            Thread.Sleep(100);
        }

        [When("I flush hard with try-catch")]
        public async Task WhenIFlushHardWithTry_Catch()
        {
            Thread.Sleep(100);
            try
            {
                await WhenIFlushHard();
            }
            catch (AggregateAssertionException ex)
            {
                ctx["FlushException"] = ex;
            }
            Thread.Sleep(100);
        }

        [Then("the soft state has already been flushed")]
        public async Task ThenTheSoftStateHasAlreadyBeenFlushed()
        {
            Thread.Sleep(100);
            var ex = ctx.Get<AggregateAssertionException>("FlushException") ?? throw new Exception("No flush exception found");
            await Stateless.Assert.That(GetAsserter().Soft.SoftState.AlreadyFlushed).IsTrue();
            Thread.Sleep(100);
        }


        [Then("the test will fail")]
        public void ThenTheTestWillFail()
        {
            Thread.Sleep(100);
            // This step is just a placeholder to indicate that the previous flush should cause a test failure.
            Thread.Sleep(100);
        }

        [Then("the soft asserter configuration has {int} callback registered")]
        public async Task ThenTheSoftAsserterConfigurationHasCallbackRegistered(int count)
        {
            await Stateless.Assert.That(GetAsserter().Soft.Settings().OnSoftFailure).HasCount(count);
        }

        // Instance Stateless asserter scenarios
        [When("I use instance hard asserter and wrap failure")]
        public async Task WhenIUseInstanceHardAsserterAndWrapFailure()
        {
            var hard = new StatefulAsserter();
            string msg = "";
            try
            {
                await hard.That(1).Is(2);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            ctx["InstanceHardAsserterMessage"] = msg;
        }

        [Then("the instance hard asserter error message should contain \"Assertion failed\"")]
        public async Task ThenTheInstanceHardAsserterErrorMessageShouldContainAssertionFailed()
        {
            var msg = ctx.Get<string>("InstanceHardAsserterMessage");
            await Stateless.Assert.That(msg).Contains("Assertion failed");
        }

        [When("I use instance hard asserter with valid assertion")]
        public async Task WhenIUseInstanceHardAsserterWithValidAssertion()
        {
            var hard = new StatefulAsserter();
            await hard.That(42).Is(42);
            ctx["InstanceHardAsserterValid"] = true;
        }

        [Then("the test should pass")]
        public async Task ThenTheTestShouldPass()
        {
            await Stateless.Assert.That(ctx.Get<bool>("InstanceHardAsserterValid")).IsTrue();
        }

        [When("I use instance hard asserter with multiple assertions")]
        public async Task WhenIUseInstanceHardAsserterWithMultipleAssertions()
        {
            var hard = new StatefulAsserter();
            await hard.That(1).Is(1);
            await hard.That("test").Is("test");
            await hard.That(true).IsTrue();
            ctx["MultipleAssertionsPass"] = true;
        }

        [Then("all assertions should pass")]
        public async Task ThenAllAssertionsShouldPass()
        {
            await Stateless.Assert.That(ctx.Get<bool>("MultipleAssertionsPass")).IsTrue();
        }

        [When("I use instance hard asserter and it fails on second assertion")]
        public async Task WhenIUseInstanceHardAsserterAndItFailsOnSecondAssertion()
        {
            var hard = new StatefulAsserter();
            string msg = "";
            try
            {
                await hard.That(1).Is(1);
                await hard.That(2).Is(999); // This should throw
                await hard.That(3).Is(3);   // Should not reach here
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            ctx["FailureMessage"] = msg;
        }

        [Then("the error message should contain both \"1\" and \"999\"")]
        public async Task ThenTheErrorMessageShouldContainBothAnd()
        {
            var msg = ctx.Get<string>("FailureMessage");
            await Stateless.Assert.That(msg).Contains("2");
            await Stateless.Assert.That(msg).Contains("999");
        }

        [When("I use instance hard asserter")]
        public void WhenIUseInstanceHardAsserter()
        {
            var instance = new StatefulAsserter();
            ctx["InstanceHardAsserter"] = instance;
        }

        [When("I use ambient hard asserter")]
        public void WhenIUseAmbientHardAsserter()
        {
            var ambient = Ambient.Assert;
            ctx["AmbientHardAsserter"] = ambient;
        }

        [Then("they should be different instances")]
        public async Task ThenTheyShouldBeDifferentInstances()
        {
            var instance = ctx.Get<StatefulAsserter>("InstanceHardAsserter");
            var ambient = ctx.Get<StatefulAsserter>("AmbientHardAsserter");
            await Stateless.Assert.That(ReferenceEquals(instance, ambient)).IsFalse();
        }

        [When("I create two instance hard asserters")]
        public void WhenICreateTwoInstanceHardAsserters()
        {
            var local1 = new StatefulAsserter();
            var local2 = new StatefulAsserter();
            ctx["HardAsserter1"] = local1;
            ctx["HardAsserter2"] = local2;
        }

        [Then("instance hard asserter and ambient hard asserter should be different instances")]
        public async Task ThenInstanceHardAsserterAndAmbientHardAsserterShouldBeDifferentInstances()
        {
            var instance = ctx.Get<StatefulAsserter>("InstanceHardAsserter");
            var ambient = ctx.Get<StatefulAsserter>("AmbientHardAsserter");
            await Stateless.Assert.That(ReferenceEquals(instance, ambient)).IsFalse();
        }

        [Then("the two instance hard asserters should be different instances")]
        public async Task ThenTheTwoInstanceHardAssertersShouldBeDifferentInstances()
        {
            var hard1 = ctx.Get<StatefulAsserter>("HardAsserter1");
            var hard2 = ctx.Get<StatefulAsserter>("HardAsserter2");
            await Stateless.Assert.That(ReferenceEquals(hard1, hard2)).IsFalse();
        }

        // Soft asserter OnFlush hook scenarios
        [When("I create soft asserter with OnFlush hook")]
        public void WhenICreateSoftAsserterWithOnFlushHook()
        {
            var hooksCalled = new List<string>();
            var verify = new SoftAsserter();
            verify.SoftState.OnFlush = [async (ex) =>
            {
                hooksCalled.Add("OnFlush");
                await Task.CompletedTask;
            }];
            ctx["HooksCalled"] = hooksCalled;
            ctx["SoftAsserterWithHook"] = verify;
        }

        [When("I add error to first soft asserter")]
        public async Task WhenIAddErrorToFirstSoftAsserter()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithHook");
            await verify.That(1).Is(2);
        }

        [Then("the OnFlush hook should have been called")]
        public async Task ThenTheOnFlushHookShouldHaveBeenCalled()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithHook");
            var hooksCalled = ctx.Get<List<string>>("HooksCalled");

            try
            {
                await verify.SoftState.FlushIfNeeded();
            }
            catch { }

            await Stateless.Assert.That(hooksCalled.Count).Is(1);
            await Stateless.Assert.That(hooksCalled[0]).Is("OnFlush");
        }

        [When("I create soft asserter with multiple OnFlush hooks")]
        public void WhenICreateSoftAsserterWithMultipleOnFlushHooks()
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
            ctx["HooksCalled"] = hooksCalled;
            ctx["SoftAsserterWithMultipleHooks"] = verify;
        }

        [When("I add error to second soft asserter")]
        public async Task WhenIAddErrorToSecondSoftAsserter()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithMultipleHooks");
            await verify.That(1).Is(2);
        }

        [Then("all hooks should have been called")]
        public async Task ThenAllHooksShouldHaveBeenCalled()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithMultipleHooks");
            var hooksCalled = ctx.Get<List<string>>("HooksCalled");

            try
            {
                await verify.SoftState.FlushIfNeeded();
            }
            catch { }

            await Stateless.Assert.That(hooksCalled.Count).Is(2);
            await Stateless.Assert.That(hooksCalled[0]).Is("Hook1");
            await Stateless.Assert.That(hooksCalled[1]).Is("Hook2");
        }

        [When("I create soft asserter with exception capture hook")]
        public void WhenICreateSoftAsserterWithExceptionCaptureHook()
        {
            var capturedExceptions = new List<AggregateAssertionException>();
            var verify = new SoftAsserter();
            verify.SoftState.OnFlush = [async (ex) =>
            {
                capturedExceptions.Add(ex);
                await Task.CompletedTask;
            }];
            ctx["CapturedExceptions"] = capturedExceptions;
            ctx["SoftAsserterWithCapture"] = verify;
        }

        [When("I add multiple errors to third soft asserter")]
        public async Task WhenIAddMultipleErrorsToThirdSoftAsserter()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithCapture");
            await verify.That(1).Is(2);
            await verify.That("a").Is("b");
        }

        [Then("the hook should have captured the aggregate exception with all errors")]
        public async Task ThenTheHookShouldHaveCapturedTheAggregateExceptionWithAllErrors()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithCapture");
            var capturedExceptions = ctx.Get<List<AggregateAssertionException>>("CapturedExceptions");

            try
            {
                await verify.SoftState.FlushIfNeeded();
            }
            catch { }

            await Stateless.Assert.That(capturedExceptions.Count).Is(1);
            await Stateless.Assert.That(capturedExceptions[0].InnerExceptions.Count).Is(2);
        }

        [When("I create soft asserter with custom flush action")]
        public void WhenICreateSoftAsserterWithCustomFlushAction()
        {
            var verify = new SoftAsserter();
            verify.SoftState.FlushAction = async (ex) =>
            {
                // Custom behavior: log instead of throw
                System.Diagnostics.Debug.WriteLine($"Soft assertions flushed: {ex.InnerExceptions.Count} errors");
                await Task.CompletedTask;
            };
            ctx["SoftAsserterWithCustomAction"] = verify;
        }

        [When("I add error to fourth soft asserter")]
        public async Task WhenIAddErrorToFourthSoftAsserter()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithCustomAction");
            await verify.That(1).Is(2);
        }

        [Then("no exception should be thrown because flush action is set")]
        public async Task ThenNoExceptionShouldBeThrownBecauseFlushActionIsSet()
        {
            var verify = ctx.Get<SoftAsserter>("SoftAsserterWithCustomAction");

            // This should not throw because FlushAction is set
            await verify.SoftState.FlushIfNeeded();

            await Stateless.Assert.That(verify.SoftState.AlreadyFlushed).IsTrue();
        }

        public enum AsserterSourceMode
        {
            Default,
            DI,
            Ambient
        }

        private const string AsserterSourceModeKey = "AsserterSourceModeKey";
        private AsserterSourceMode GetAsserterSourceMode()
        {
            return ctx.Get<AsserterSourceMode>(AsserterSourceModeKey);
        }
        private StatefulAsserter GetAsserter()
        {
            var mode = GetAsserterSourceMode();
            if (mode == AsserterSourceMode.DI)
            {
                return assert;
            }
            else if (mode == AsserterSourceMode.Ambient)
            {
                return Ambient.Assert;
            }
            throw new Exception("No AsserterSourceMode found");
        }
    }
}
