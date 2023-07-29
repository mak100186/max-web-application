﻿@Acceptance @freebet
Feature: ClaimSettle

Background:
    Given a bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' for customer '918737' that has formulae 'singles' and stake '14' with the combinations:
    | combinationRn                                              | selectionOutcomes |
    | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | barcaWin          |
    And the bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' has the following stages:
    | outcome  | price | market                                                        |
    | barcaWin | 1.2   | ksp:market.1:[basketball:201711202200:barca_vs_liverpool]:1x2 |

@singles
Scenario: Settling a claimed FreeBet for a singles without previous settlement should return the negative of the combination stake when the bet payoff is greater than or equal to the freebet amount 
    Given I have bonuses in the system
        | name              | rewardType | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id | amount |
        | uniboost_reward_1 | freebet    | singles         | 1         | 1         | 1               | 1               | 918737      | 5      |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      |               |
    When a request to claim an entitlement is received for customer '918737':
        | name              | betRn                                            |
        | uniboost_reward_1 | ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1 |
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
        | -14    | 0          |

@singles
Scenario: Settling a claimed FreeBet for a singles without previous settlement should return the negative of the bet payoff when the bet payoff is less than freebet amount 
    Given I have bonuses in the system
        | name              | rewardType | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id | amount |
        | uniboost_reward_1 | freebet    | singles         | 1         | 1         | 1               | 1               | 918737      | 5      |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      |               |
    When a request to claim an entitlement is received for customer '918737':
        | name              | betRn                                            |
        | uniboost_reward_1 | ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918737':
        | name   |
        | 918737 |
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '2' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | -2     | 0          |

@singles
Scenario: Settling a claimed FreeBet for a singles without previous settlement should return 0 when the betpayoff is 0
    Given I have bonuses in the system
        | name              | rewardType | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id | amount |
        | uniboost_reward_1 | freebet    | singles         | 1         | 1         | 1               | 1               | 918737      | 5      |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      |               |
    When a request to claim an entitlement is received for customer '918737':
        | name              | betRn                                            |
        | uniboost_reward_1 | ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918737':
        | name   |
        | 918737 |
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '0' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Won             |
    Then the HttpStatusCode should be 200
    And the response should be:
        | payOff | prevPayOff |
        | 0      | 0          |
