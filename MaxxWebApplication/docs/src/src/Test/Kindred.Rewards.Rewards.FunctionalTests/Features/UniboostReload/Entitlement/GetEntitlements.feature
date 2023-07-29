@Acceptance @uniboostreload
Feature: GetEntitlements

Background:
    Given a bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' for customer '123456' that has formulae 'singles' and stake '14' with the combinations:
        | combinationRn                                              | selectionOutcomes   |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | barcaWin,brazilLose |
    And the bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' has the following stages:
        | outcome  | price | market                                                      |
        | barcaWin | 1.2   | ksp:market.1:[football:201711202200:barca_vs_liverpool]:1x2 |
    
    Given I have bonuses in the system
        | name                    | rewardType     | allowedFormulae | minStages | maxStages | minCombinations | maxCombinations | customer_id | claimsPerInterval | maxReload | stopOnMinimumWinBets |
        | uniboostreload_reward_1 | uniboostreload | singles         | 1         | 1         | 1               | 1               | 123456      | 2                 | 3         | 2                    |
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 123456      |               |
    And a request to claim an entitlement is received for customer '123456':
        | name                    | betRn                                            |
        | uniboostreload_reward_1 | ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1 |

@singles
Scenario: When entitlements are retrieved for UniBoostReload which had an associated lost bet remaining claims should not be reduced
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Lost            |
    And I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 123456      |               |
    Then the entitlements for customer '123456' should contain:
        | name                    | remainingClaimsPerInterval |
        | uniboostreload_reward_1 | 2                          |

@singles
Scenario: When entitlements are retrieved for UniBoostReload which had an associated won bet remaining claims should be reduced
    When a request to settle the claim is received for bet 'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1' with final payoff '12' and combination settlement statuses:
        | combinationRn                                              | settlementStatus | segmentStatuses |
        | ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1 | Resolved         | Won             |
    And I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 123456      |               |
    Then the entitlements for customer '123456' should contain:
        | name                    | remainingClaimsPerInterval |
        | uniboostreload_reward_1 | 1                          |
