using Catchy.Sdk;
using Catchy;

namespace AmbientReqnrollTests.StepDefinitions
{
    [Binding]
    public class CombinationTestsStepDefinitions(StatefulAsserter assert, ScenarioContext ctx)
    {
        private const string AsserterSourceModeKey = "AsserterSourceModeKey";
        private const string SoftInstance1Key = "SoftInstance1";
        private const string SoftInstance2Key = "SoftInstance2";
        private const string CustomAsserterKey = "CustomAsserter";

        public enum AsserterSourceMode
        {
            Default,
            DI,
            Ambient
        }

        [When("I perform stateless hard assertion that passes")]
        public async Task WhenIPerformStatelessHardAssertionThatPasses()
        {
            await Stateless.Assert.That(1).Is(1);
        }

        [When("I perform stateful hard assertion that passes")]
        public async Task WhenIPerformStatefulHardAssertionThatPasses()
        {
            await GetAsserter().That(2).Is(2);
        }

        [When("I perform soft assertion that passes")]
        public async Task WhenIPerformSoftAssertionThatPasses()
        {
            await GetAsserter().Soft.That(3).Is(3);
        }

        [Then("no errors should have occurred")]
        public async Task ThenNoErrorsShouldHaveOccurred()
        {
            await Stateless.Assert.That(GetAsserter().Soft.HasFailures).IsFalse();
        }

        [When("I perform hard assertion that passes")]
        public async Task WhenIPerformHardAssertionThatPasses()
        {
            await GetAsserter().That(1).Is(1);
        }

        [Then("soft should have no failures")]
        public async Task ThenSoftShouldHaveNoFailures()
        {
            await Stateless.Assert.That(GetAsserter().Soft.HasFailures).IsFalse();
        }

        [Then("hard assertion should have passed")]
        public async Task ThenHardAssertionShouldHavePassed()
        {
            // Hard assertions throw on failure, so if we reach here, they passed
        }

        [When("I perform multiple soft assertions that all pass")]
        public async Task WhenIPerformMultipleSoftAssertionsThatAllPass()
        {
            await GetAsserter().Soft.That(1).Is(1);
            await GetAsserter().Soft.That("a").Is("a");
            await GetAsserter().Soft.That(true).Is(true);
        }

        [Then("soft should have no errors")]
        public async Task ThenSoftShouldHaveNoErrors()
        {
            await Stateless.Assert.That(GetAsserter().Soft).Errors().HasCount(0);
        }

        [When("I access soft assertion instance")]
        public void WhenIAccessSoftAssertionInstance()
        {
            var verify = GetAsserter().Soft;
            ctx[SoftInstance1Key] = verify;
        }

        [When("I access soft assertion instance again")]
        public void WhenIAccessSoftAssertionInstanceAgain()
        {
            var verify = GetAsserter().Soft;
            ctx[SoftInstance2Key] = verify;
        }

        [Then("soft instances should be the same")]
        public async Task ThenSoftInstancesShouldBeTheSame()
        {
            var soft1 = ctx.Get<SoftAsserter>(SoftInstance1Key);
            var soft2 = ctx.Get<SoftAsserter>(SoftInstance2Key);
            await Stateless.Assert.That(ReferenceEquals(soft1, soft2)).IsTrue();
        }

        [Then("soft error count should be {int}")]
        public async Task ThenSoftErrorCountShouldBe(int count)
        {
            await Stateless.Assert.That(GetAsserter().Soft).Errors().HasCount(count);
        }

        [When("I create a custom stateful asserter")]
        public void WhenICreateACustomStatefulAsserter()
        {
            var customAsserter = Asserter.NewStateful();
            ctx[CustomAsserterKey] = customAsserter;
        }

        [When("I perform soft assertion failures with custom asserter")]
        public async Task WhenIPerformSoftAssertionFailuresWithCustomAsserter()
        {
            var customAsserter = ctx.Get<StatefulAsserter>(CustomAsserterKey);
            await customAsserter.Soft.That(1).Is(2);
            await customAsserter.Soft.That(3).Is(4);
        }

        [Then("custom stateful error count should be {int}")]
        public async Task ThenCustomStatefulErrorCountShouldBe(int count)
        {
            var customAsserter = ctx.Get<StatefulAsserter>(CustomAsserterKey);
            await Stateless.Assert.That(customAsserter.Soft).Errors().HasCount(count);
        }

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
