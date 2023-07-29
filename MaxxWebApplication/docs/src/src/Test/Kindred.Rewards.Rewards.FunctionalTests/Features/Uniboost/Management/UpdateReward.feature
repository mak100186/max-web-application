Feature: Updating rewards

[PATCH] /reward/{rewardRn}
[PUT] /reward/{rewardRn}

Background:
    Given I have bonuses in the system
        | name                  | rewardType | customer_id | expiryDaysFromNow                       | countryCode | startDaysFromNow | comments                      |
        | uniboost_5_pct_bonus  | uniboost   | 321         |                                         | AUS         |                  |                               |
        | uniboost_6_pct_bonus  | uniboost   | 123         |                                         | RUS         |                  |                               |
        | uniboost_7_pct_bonus  | uniboost   | 123         |                                         | PAK         | 1                |                               |
        | uniboost_8_pct_bonus  | uniboost   | 123         |                                         | USA         |                  |                               |
        | uniboost_9_pct_bonus  | uniboost   | 123         | time-in-next-daylight-saving-time-shift | CHN         |                  |                               |
        | uniboost_4_pct_bonus  | uniboost   | 123         |                                         | FRA         | 1                |                               |
        | uniboost_3_pct_bonus  | uniboost   | 123         | 20                                      | FRA         | 10               |                               |
        | uniboost_10_pct_bonus | uniboost   | 123         |                                         | BEL         |                  | comment_uniboost_10_pct_bonus |

        
@bonus
Scenario: Update an existing bonus
    When I submit UpdateBonus request for 'uniboost_7_pct_bonus'
    Then the HttpStatusCode should be 200
    When I submit GetBonus request for 'uniboost_7_pct_bonus'
    Then the HttpStatusCode should be 200
    And the UpdateBonus response should return updated bonus

@bonus
Scenario: Update an existing bonus with invalid parameters
    When I submit UpdateBonus request for 'uniboost_7_pct_bonus' with invalid parameters
    Then the HttpStatusCode should be 400
    And Response should contain error ''Max Stake Amount' must be greater than '0''


@bonus
Scenario: Updating an existing bonus with a new customer ID should be disallowed
    When I submit UpdateBonus request for 'uniboost_9_pct_bonus' with '3333' as Customer ID
    Then the HttpStatusCode should be 400
    And Response should contain error 'CustomerId of an existing reward cannot be updated'

@bonus
Scenario: Patching an existing bonus with null patch request should not update anything
    When I submit a null patch bonus request for 'uniboost_10_pct_bonus'
    Then the HttpStatusCode should be 200
    And the patch bonus response should return updated bonus with the following
        | comments                      | name                  |
        | comment_uniboost_10_pct_bonus | uniboost_10_pct_bonus |

@bonus
Scenario: Patching an existing bonus with patch request should update fields
    When I submit a patch bonus request for the following criteria for 'uniboost_10_pct_bonus'
        | comments | name           |
        | hello    | my-reward-name |
    Then the HttpStatusCode should be 200
    And the patch bonus response should return updated bonus with the following
        | comments | name           |
        | hello    | my-reward-name |
