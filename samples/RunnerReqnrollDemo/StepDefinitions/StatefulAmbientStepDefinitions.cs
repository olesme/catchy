using Catchy;
using Reqnroll;

namespace RunnerReqnrollDemo.StepDefinitions;

[Binding]
public sealed class StatefulAmbientStepDefinitions(StatefulAsserter asserter, ScenarioContext scenarioContext)
    : StatefulStepsBase(asserter, scenarioContext)
{
    [Given("a valid stateful assertion context")]
    public async Task GivenAValidStatefulAssertionContext()
    {
        await Assert.That(1).Is(1);
    }

    [When("a soft assertion is captured")]
    public async Task WhenASoftAssertionIsCaptured()
    {
        await Assert.Soft.That("reqnroll").Is("reqnroll");
    }

    [Then("the scenario soft state remains valid")]
    public async Task ThenTheScenarioSoftStateRemainsValid()
    {
        await Assert.That().SoftState().HasNoErrors();
    }
}
