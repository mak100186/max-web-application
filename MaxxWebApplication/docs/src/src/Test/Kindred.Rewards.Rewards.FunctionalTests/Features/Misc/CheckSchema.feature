@Acceptance
Feature: CheckSchema

When we check for the schema, we should get yaml or json files

@repository
Scenario: Get all the schema in json and yaml format
	Given A request to the /schema/messages endpoint
	When The request is submitted
	Then I should receive a response of status code 200
    And  I expect all populated list of message schemas
