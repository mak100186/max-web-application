@Acceptance @profitboost
Feature: ClaimSettle

Background:
    Given a bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' for customer '123456' that has formulae 'singles' and stake '14' with the combinations:
    | combinationRn                                              | selectionOutcomes             |
    | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | barcaWin,brazilLose           |
    And the bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' has the following stages:
    | outcome            | price | market                                                       |
    | barcaWin           | 1.2   | ksp:market.1:[football:201711202200:barca_vs_liverpool]:1x2  |
    And a bet 'ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1' for customer '123456' that has formulae 'doubles' and stake '14' with the combinations:
    | combinationRn                                              | selectionOutcomes             |
    | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:1 | barcaWin,brazilLose           |
    | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:2 | barcaWin,australiaFourGoals   |
    | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:3 | australiaFourGoals,brazilLose |
    And the bet 'ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1' has the following stages:
    | outcome            | price | market                                                       |
    | barcaWin           | 1.2   | ksp:market.1:[football:201711202200:barca_vs_liverpool]:1x2  |
    | brazilLose         | 1.134 | ksp:market.1:[football:201711202200:brazil_vs_australia]:1x2 |
    | australiaFourGoals | 1.136 | ksp:market.1:[football:201711202200:brazil_vs_australia]:1x2 |

@singles
Scenario: Settling a claimed ProfitBoost for a singles without previous settlement should return previous as zero in first response
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
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | 0.06   | 0          |

 @singles
Scenario: Settling a claimed ProfitBoost for a single that starts resolved goes to pending and then back to resolved
    Given I have bonuses in the system
        | name                 | rewardType  | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id |
        | profitboost_reward_1 | profitboost | singles         | 1         | 1         | 1               | 1               | 918737      |
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
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | 0.06   | 0          |
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Pending          | Unresolved      |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | 0      | 0.06       |
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | 0.06   | 0          |

@doubles
Scenario: Settling a claimed ProfitBoost for a doubles with previous settlement should return prev payoff as the previous cumulative reward payoff
    Given I have bonuses in the system
        | name                 | rewardType  | legTable                    | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id |
        | profitboost_reward_1 | profitboost | {"1":"10","2":"10","4":"0"} | doubles         | 2         | 20        | 2               | 2               | 918737      |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      |               |
    When a request to claim an entitlement is received for customer '918737':
        | name                 | betRn                                            |
        | profitboost_reward_1 | ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918737':
        | name   |
        | 918737 |
    When a request to settle the claim is received for bet 'ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:1 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff  | prevPayOff |
        | 0.10824 | 0          |
    When a request to settle the claim is received for bet 'ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:2 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff  | prevPayOff |
        | 0.21720 | 0.10824    |  

@doubles
Scenario: Settling a claimed ProfitBoost with one combination Lost and the second Won should return zero for first response
    Given I have bonuses in the system
        | name                 | rewardType  | legTable                    | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id |
        | profitboost_reward_1 | profitboost | {"1":"10","2":"10","4":"0"} | doubles         | 2         | 20        | 2               | 2               | 918737      |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      |               |
    When a request to claim an entitlement is received for customer '918737':
        | name                 | betRn                                            |
        | profitboost_reward_1 | ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918737':
        | name   |
        | 918737 |
    When a request to settle the claim is received for bet 'ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:1 | Resolved         | Lost            |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | 0      | 0          |
    When a request to settle the claim is received for bet 'ksp:bet.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:75e1f14e-4181-4db6-8e5f-c255a4ee0672:1:2 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff  | prevPayOff |
        | 0.10896 | 0          |
