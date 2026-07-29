Feature: Combination Error Tests for Soft Assertions
  In order to verify error accumulation and soft failure behaviors
  As a developer testing Catchy
  I want to ensure soft assertions accumulate correctly and auto-flush on test cleanup

Rule: Expected-to-fail scenarios with ambient asserter
Background:
	Given asserter source is Ambient

@xfail
Scenario: [XFAIL] Soft accumulates multiple errors and fails on flush
	When I got soft fail
	And I got soft fail
	And I got soft fail
	Then the test will fail

@xfail
Scenario: [XFAIL] Hard throws but soft accumulates
	When I got soft fail
	And I use hard asserter that throws
	Then the test will fail

@xfail
Scenario: [XFAIL] Soft errors are instance-specific
	When I got soft fail
	Then the soft fails count should be 1
	And the test will fail

@xfail
Scenario: [XFAIL] Hard exception caught but soft continues
	When I got soft fail
	And I use hard asserter that throws
	Then the soft fails count should be 1
	And the test will fail

@xfail
Scenario: [XFAIL] Soft state persists across multiple accesses
	When I got soft fail
	And I got soft fail
	Then the soft fails count should be 2
	And the test will fail

@xfail
Scenario: [XFAIL] Single soft error is flushed
	When I got soft fail
	Then the test will fail
