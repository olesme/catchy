Feature: Soft Assertions feature
  In order to be able to use soft assertions in Catchy
  As a user of Catchy
  I want to be able to use soft assertions in my tests

Rule: Sceanrios using DI asserter source
Background:
	Given asserter source is DI

@xfail
Scenario: [XFAIL] Checking explicit flush
	When I got soft fail
	And I flush hard
	Then the test will fail

@xfail
Scenario: [XFAIL] Checking auto flush
	When I got soft fail
	Then the test will fail

Rule: Sceanrios using ambient asserter source
Background:
	Given asserter source is Ambient
	
@xfail
Scenario: [XFAIL] Checking ambient explicit flush
	When I got soft fail
	And I flush hard
	Then the test will fail

@xfail
Scenario: [XFAIL] Checking ambient auto flush
	When I got soft fail
	Then the test will fail
