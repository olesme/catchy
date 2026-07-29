using Catchy;
using Reqnroll;

namespace RunnerReqnrollDemo.StepDefinitions;

public abstract class StatefulStepsBase(StatefulAsserter asserter, ScenarioContext scenarioContext)
{
    protected StatefulAsserter Assert { get; } = asserter;
    protected ScenarioContext ScenarioContext { get; } = scenarioContext;
}
