Feature: Cancelling rewards

[PUT] rewards/{rewardRn}/cancel

Background:
    Given I have bonuses in the system
        | name                 | rewardType | customer_id | expiryDaysFromNow | countryCode | startDaysFromNow | comments |
        | uniboost_8_pct_bonus | uniboost   | 123         |                   | USA         |                  |          |


Scenario: Cancel an existing bonus
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 123         | test          |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | bonus_name           | promotion_name |
        | uniboost_8_pct_bonus |                |

    When I submit CancelBonus request for 'uniboost_8_pct_bonus'
    Then the HttpStatusCode should be 200
    When I submit GetBonus request for 'uniboost_8_pct_bonus'
    Then the HttpStatusCode should be 200
    And the CancelBonus response should return cancelled bonus
    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id |
        | true             | true           | true          | 123         |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should return bonus 'uniboost_8_pct_bonus'
    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id |
        | true             | false          | false         | 123         |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should return bonus 'uniboost_8_pct_bonus'
    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id |
        | false            | true           | true          | 123         |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should not return bonus 'uniboost_8_pct_bonus'

    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 123         | test          |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should not include following rewards:
        | bonus_name           | promotion_name |
        | uniboost_8_pct_bonus |                |
