Feature: GetReward

A short summary of the feature

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
Scenario: Get valid bonus
    When I submit GetBonus request for 'uniboost_5_pct_bonus'
    Then the HttpStatusCode should be 200
    And the GetBonus response should return bonus 'uniboost_5_pct_bonus'

@bonus
Scenario: Get bonus that does not exist throws 404
    When I submit GetBonus request for reward key '8768c4e1-e897-435f-af02-c255ffd27a64'
    Then the HttpStatusCode should be 404
    And Response should contain error 'Could not find a reward with the provided reward key 8768c4e1-e897-435f-af02-c255ffd27a64'

@bonus
Scenario: Get bonus with rewardRn that is not a valid Rn throws 400
    When I submit GetBonus request for reward key 'blah'
    Then the HttpStatusCode should be 400
    And Response should contain error 'Could not parse the provided RewardRns. Supported values are the Rn identifier or the full Rn. Provided Rn: blah"'
