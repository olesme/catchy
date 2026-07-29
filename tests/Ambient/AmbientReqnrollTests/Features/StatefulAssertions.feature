Feature: Stateful Assertions feature
  In order to be able to use stateful assertions in Catchy
  As a user of Catchy
  I want to be able to use stateful assertions in my tests

Rule: Sceanrios using DI asserter source
Background:
	Given asserter source is DI

Scenario: Checking soft assertions count
	When I got soft fail
	Then the soft fails count should be 1
	When I cleanup soft fails
	Then the soft fails count should be 0

Scenario: Checking explicit flush with try-catch
	When I got soft fail
	And I flush hard with try-catch
	Then the soft state has already been flushed

Rule: Sceanrios using ambient asserter source
Background:
	Given asserter source is Ambient

Scenario: Checking ambient soft assertions count
	When I got soft fail
	Then the soft fails count should be 1
	When I cleanup soft fails
	Then the soft fails count should be 0

Rule: Instance Hard asserter scenarios
Scenario: Instance hard asserter wraps failures successfully
	When I use instance hard asserter and wrap failure
	Then the instance hard asserter error message should contain "Assertion failed"

Scenario: Instance hard asserter passes valid assertions
	When I use instance hard asserter with valid assertion
	Then the test should pass

Scenario: Instance hard asserter multiple assertions
	When I use instance hard asserter with multiple assertions
	Then all assertions should pass

Scenario: Instance hard asserter throws on first failure
	When I use instance hard asserter and it fails on second assertion
	Then the error message should contain both "1" and "999"

Scenario: Instance hard asserter isolated from ambient
	When I use instance hard asserter
	And I use ambient hard asserter
	Then instance hard asserter and ambient hard asserter should be different instances

Scenario: Instance hard asserter isolated from other instances
	When I create two instance hard asserters
	Then the two instance hard asserters should be different instances

Rule: Soft asserter OnFlush hook scenarios
Scenario: Soft asserter OnFlush hook called on failure
	When I create soft asserter with OnFlush hook
	And I add error to first soft asserter
	Then the OnFlush hook should have been called

Scenario: Soft asserter multiple OnFlush hooks
	When I create soft asserter with multiple OnFlush hooks
	And I add error to second soft asserter
	Then all hooks should have been called

Scenario: Soft asserter hook receives aggregate exception
	When I create soft asserter with exception capture hook
	And I add multiple errors to third soft asserter
	Then the hook should have captured the aggregate exception with all errors

Scenario: Soft asserter hook can modify behavior
	When I create soft asserter with custom flush action
	And I add error to fourth soft asserter
	Then no exception should be thrown because flush action is set
