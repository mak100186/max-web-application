@Acceptance @profitboost
Feature: Claim

Background:
    Given a bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' for customer '123456' that has formulae 'singles' and stake '14' with the combinations:
    | combinationRn                                              | selectionOutcomes             |
    | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | barcaWin,brazilLose           |
    And the bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' has the following stages:
    | outcome            | price | market                                                       |
    | barcaWin           | 1.2   | ksp:market.1:[football:201711202200:barca_vs_liverpool]:1x2  |

@singles
Scenario: Claiming a ProfitBoost for a singles should succeed
    Given I have bonuses in the system
        | name                 | rewardType  | legTable                    | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id |
        | profitboost_reward_1 | profitboost | {"1":"10","2":"10","4":"0"} | singles         | 1         | 1         | 1               | 1               | 918737      |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      |               |
    When a request to claim an entitlement is received for customer '918737':
        | name                 | betRn                                            |
        | profitboost_reward_1 | ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918737':
        | name   |
        | 918737 |
