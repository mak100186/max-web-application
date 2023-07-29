@Acceptance @variants @minimumodds @profitboost @ignore
Feature: ProfitboostBonusWithMinimumOddsConfigured

Background:
    Given I have promotions templates in the system
        | key                    | comments               |
        | profitboost_bonus_user | profitboost_bonus_user |
    Then the HttpStatusCode should be 201
    Given I have bonuses in the system
        | name              | rewardType  | legTable                    | allowedFormulae | customer_id | minStages | maxStages | minCombinations | maxCombinations | minimumStageOdds |
        | profitboost_bonus | profitboost | {"1":"10","2":"10","4":"0"} | singles,doubles | 918792      | 1         | 15        | 1               | 2               | 1.5              |
    Then the HttpStatusCode should be 201
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys          |
        | 918792      | profitboost_bonus_user |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | bonus_name        | promotion_name |
        | profitboost_bonus |                |

Scenario: Claim with 1 stage that is above the configured minimum stage odds
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData |
        | 918792      | profitboost_bonus | 1                | 1.5            |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918792':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918792      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 0.15

Scenario: Claim with 1 stage that is below the configured minimum stage odds
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData |
        | 918792      | profitboost_bonus | 1                | 1              |
    Then the HttpStatusCode should be 400
    And I expect following in the ClaimedEntitlements for the customerID='918792':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918792      |
    Then the HttpStatusCode should be 400
    And Response should contain error 'No stages adhere to the configured minimum stage odds: 1.5'
