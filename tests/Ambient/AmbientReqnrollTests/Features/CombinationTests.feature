Feature: Combination Tests for Stateless, Stateful, and Soft Assertions
  In order to verify that different assertion modes work correctly together
  As a developer testing Catchy
  I want to ensure stateless hard, stateful hard, and soft assertions are independent

Rule: Combination scenarios with ambient asserter
Background:
	Given asserter source is Ambient

Scenario: Stateless and stateful and soft assertions all work together
	When I perform stateless hard assertion that passes
	And I perform stateful hard assertion that passes
	And I perform soft assertion that passes
	Then no errors should have occurred

Scenario: Soft failures do not affect hard assertions
	When I perform soft assertion that passes
	And I perform hard assertion that passes
	Then soft should have no failures
	And hard assertion should have passed

Scenario: Multiple soft errors accumulate correctly
	When I perform multiple soft assertions that all pass
	Then soft should have no errors

Scenario: Interleaved hard and soft assertions work together
	When I perform hard assertion that passes
	And I perform soft assertion that passes
	And I perform hard assertion that passes
	And I perform soft assertion that passes
	Then soft should have no failures

Scenario: Stateful soft returns same instance
	When I access soft assertion instance
	And I perform soft assertion that passes
	And I access soft assertion instance again
	Then soft instances should be the same
	And soft error count should be 0

Scenario: Soft state inspection doesnt flush
	When I perform soft assertion that passes
	Then soft should have no failures
	And soft error count should be 0

Scenario: Custom stateful soft accumulates but no auto flush
	When I create a custom stateful asserter
	And I perform soft assertion failures with custom asserter
	Then custom stateful error count should be 2
