Feature: ClaimReward

[POST] /claims/batchclaim

Background:
    Given I have promotions templates in the system
        | key      | comments          |
        | new_user | new user template |
        | vip_user | vip new template  |

@claim
Scenario: Claim entitlements
    When I create Bonus for following criteria:
        | customer_id | name |
        | 918737      | 123  |
    Then the HttpStatusCode should be 201
    When I submit CreatePromotion request for following promotions:
        | promotion_name | claim_limit |
        | vip_uniboost   | 1           |
    Then the HttpStatusCode should be 201
    When I submit CreateMapping request for following promotions:
        | template_key | promotions   |
        | vip_user     | vip_uniboost |
    Then the HttpStatusCode should be 200
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      | vip_user      |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name | bonus_name | customer_id |
        | vip_uniboost   | 123        | 918737      |
    When I submit ClaimEntitlement request for following criteria:
        | customer_id | name         | number_of_claims | contestKey |
        | 918737      | vip_uniboost | 1                | default    |
        | 918737      | 123          | 1                | default    |
    Then the HttpStatusCode should be 200
    And ClaimEntitlement response should claim following rewards:
        | promotion_name | bonus_name | customer_id |
        | vip_uniboost   | 123        | 918737      |

@claim
Scenario: Claim entitlements - outcome is not allowed
    When I create Bonus for following criteria:
        | customer_id | name | filterOutcome |
        | 918737      | 123  | plain:draw    |
    Then the HttpStatusCode should be 201
    When I submit CreatePromotion request for following promotions:
        | promotion_name | claim_limit |
        | vip_uniboost   | 1           |
    Then the HttpStatusCode should be 201
    When I submit CreateMapping request for following promotions:
        | template_key | promotions   |
        | vip_user     | vip_uniboost |
    Then the HttpStatusCode should be 200
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      | vip_user      |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name | bonus_name | customer_id |
        | vip_uniboost   | 123        | 918737      |
    When I submit ClaimEntitlement request for following criteria:
        | customer_id | name         | number_of_claims | outcome                                                                                                          | contestKey |
        | 918737      | vip_uniboost | 1                | ksp:outcome.1:[football:202304210500:shandong_luneng_srl_vs_nantong_zhiyun_srl]:highest_scoring_half:plain:equal | default    |
        | 918737      | 123          | 1                | ksp:outcome.1:[football:202304210500:shandong_luneng_srl_vs_nantong_zhiyun_srl]:highest_scoring_half:plain:equal | default    |
    Then the HttpStatusCode should be 400

@claim
Scenario: Claim entitlements - outcome is allowed
    When I create Bonus for following criteria:
        | customer_id | name | filterOutcome |
        | 918737      | 123  | plain:draw    |
    Then the HttpStatusCode should be 201
    When I submit CreatePromotion request for following promotions:
        | promotion_name | claim_limit |
        | vip_uniboost   | 1           |
    Then the HttpStatusCode should be 201
    When I submit CreateMapping request for following promotions:
        | template_key | promotions   |
        | vip_user     | vip_uniboost |
    Then the HttpStatusCode should be 200
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918737      | vip_user      |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name | bonus_name | customer_id |
        | vip_uniboost   | 123        | 918737      |
    When I submit ClaimEntitlement request for following criteria:
        | customer_id | name         | number_of_claims | outcome                                                                                                         | contestKey |
        | 918737      | vip_uniboost | 1                | ksp:outcome.1:[football:202304210500:shandong_luneng_srl_vs_nantong_zhiyun_srl]:highest_scoring_half:plain:draw | default    |
        | 918737      | 123          | 1                | ksp:outcome.1:[football:202304210500:shandong_luneng_srl_vs_nantong_zhiyun_srl]:highest_scoring_half:plain:draw | default    |
    Then the HttpStatusCode should be 200
    And ClaimEntitlement response should claim following rewards:
        | promotion_name | bonus_name | customer_id |
        | vip_uniboost   | 123        | 918737      |

@claim
Scenario: Claim entitlements with given reward terms
    When I submit CreatePromotion request for following promotions:
        | promotion_name | claim_limit | claim_interval   |
        | vip_uniboost   | 2           | 0/30 * * ? * * * |
    Then the HttpStatusCode should be 201
    When I submit CreateMapping request for following promotions:
        | template_key | promotions   |
        | vip_user     | vip_uniboost |
    Then the HttpStatusCode should be 200
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918738      | vip_user      |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name | bonus_name |
        | vip_uniboost   |            |
    When I submit ClaimEntitlement request for following criteria:
        | customer_id | name         | number_of_claims | contestKey |
        | 918738      | vip_uniboost | 2                | default    |
    Then the HttpStatusCode should be 200
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys |
        | 918738      | vip_user      |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should not include promotion 'vip_uniboost'
    When I submit GetEntitlement request after the next interval for following criteria:
        | customer_id | template_keys | claim_interval   |
        | 918738      | vip_user      | 0/30 * * ? * * * |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name | bonus_name |
        | vip_uniboost   |            |
