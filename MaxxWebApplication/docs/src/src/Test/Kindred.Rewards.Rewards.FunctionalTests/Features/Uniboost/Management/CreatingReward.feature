Feature: Creating rewards

[POST] /reward/uniboost
[POST] /reward/uniboost/systemrewards

Scenario: Creating reward
    When I create a reward 
        | name                  | rewardType | customer_id |
        | uniboost_5_pct_bonus  | uniboost   | 321         |
    Then I expect RewardCreated message with reward type 'Uniboost' and customerId '321'
