@Acceptance @ignore
Feature: ProfitboostBonus

Background:
    Given I have promotions templates in the system
        | key                    | comments               |
        | profitboost_bonus_user | profitboost_bonus_user |
    Then the HttpStatusCode should be 201
    Given I have bonuses in the system
        | name                | rewardType  | legTable                              | allowedFormulae | customer_id | minStages | maxStages | minCombinations | maxCombinations | minimumStageOdds |
        | profitboost_bonus   | profitboost | {"1":"10","2":"10","4":"0"}           | singles,doubles | 918791      | 1         | 15        | 1               | 2               |                  |
        | profitboost_bonus_2 | profitboost | {"2":"10","4":"15","7":"20","10":"6"} | singles         | 918791      | 2         | 15        | 1               | 1               |                  |
    Then the HttpStatusCode should be 201
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys          |
        | 918791      | profitboost_bonus_user |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | bonus_name                            | promotion_name |
        | profitboost_bonus,profitboost_bonus_2 |                |

@profitboost
Scenario: Profitboost - Settle 1 stage claim for reward applicable to Single and Standard multi
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData |
        | 918791      | profitboost_bonus | 1                | 1.5            |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 0.15

@profitboost
Scenario: Profitboost - Settle 4 stage claim for reward applicable to Single and Standard multi and boost is configured as zero for four legs.
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData  |
        | 918791      | profitboost_bonus | 1                | 1.5,1.5,2.0,1.1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 0

@profitboost
Scenario: Profitboost - Settle 3 stage claim for reward applicable to Single and Standard multi
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData |
        | 918791      | profitboost_bonus | 1                | 1.5,1.5,2.0    |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 1.050

@profitboost
Scenario: Profitboost - Settle 3 stage claim for reward applicable to Single and Standard multi with maxWin cap applied
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData |
        | 918791      | profitboost_bonus | 1                | 1.5,10.5,20.0  |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 94.20

@profitboost
Scenario: Profitboost - Settle 2 stage claim for reward applicable to Standard multi
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name                | number_of_claims | stagePriceData |
        | 918791      | profitboost_bonus_2 | 1                | 1.5,1.5        |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name                |
        | profitboost_bonus_2 |
    When I settle the bet for following claim
        | name                | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus_2 | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 0.375

@profitboost
Scenario: Profitboost - Settle 7 stage claim for reward applicable to Standard multi
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name                | number_of_claims | stagePriceData              |
        | 918791      | profitboost_bonus_2 | 1                | 1.5,1.5,2.0,1.1,1.3,1.1,2.0 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name                |
        | profitboost_bonus_2 |
    When I settle the bet for following claim
        | name                | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus_2 | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 7.894200

@profitboost
Scenario: Profitboost - Settle 4 stage claim for reward applicable to Standard multi
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name                | number_of_claims | stagePriceData  |
        | 918791      | profitboost_bonus_2 | 1                | 1.5,1.5,2.0,1.1 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name                |
        | profitboost_bonus_2 |
    When I settle the bet for following claim
        | name                | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus_2 | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 1.77750

@profitboost
Scenario: Profitboost - Settle 11 stage claim for reward applicable to Standard multi with maxWin cap applied
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name                | number_of_claims | stagePriceData                              |
        | 918791      | profitboost_bonus_2 | 1                | 1.5,1.5,2.0,1.1,1.3,1.1,2.0,1.1,1.3,1.1,2.0 |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name                |
        | profitboost_bonus_2 |
    When I settle the bet for following claim
        | name                | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus_2 | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 7.8368259600

@profitboost
Scenario: Profitboost - Settle 20 stage claim should not be allowed due to ProfitBoost configured to have maximum of 15 stages
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name                | number_of_claims | stagePriceData                                                                  |
        | 918791      | profitboost_bonus_2 | 1                | 1.5,1.5,2.0,1.1,1.3,1.1,2.0,1.1,1.3,1.1,2.0,1.2,1.3,1.7,1.8,1.4,1.9,1.9,2.0,1.6 |
    Then the HttpStatusCode should be 400
    And Response should contain error 'Multi bet has 20 stages which is not in allowed range: [2-15]'

@profitboost
Scenario: Profitboost - Unsettle pathway
    When I submit ClaimEntitlement request for following criteria for Currency 'AUD':
        | customer_id | name              | number_of_claims | stagePriceData |
        | 918791      | profitboost_bonus | 1                | 1.5            |
    Then the HttpStatusCode should be 200
    And I expect following in the ClaimedEntitlements for the customerID='918791':
        | name              |
        | profitboost_bonus |
    When I settle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Winning     | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimSettledEvent
    And I expect the correct Payout with rewardPaymentAmount of 0.15
    When I unsettle the bet for following claim
        | name              | bet_outcome | stake_amount | return_as_freebet | customer_id |
        | profitboost_bonus | Losing      | 3.00         | false             | 918791      |
    Then I expect to receive RewardClaimUnSettledEvent
    And I expect a reverse payment of the same payout amount
