Feature: GetEntitlements

[GET] /claims/entitlements/{customerId}

Background:
    Given I have promotions templates in the system
        | key                 | comments            |
        | uniboost_bonus_user | uniboost_bonus_user |
        | uniboost_promo_user | uniboost_promo_user |
        | new_user            | new user template   |
        | vip_user            | vip new template    |
    Then the HttpStatusCode should be 201

Scenario: GetEntitlement request returns all rewards linked to customer
    Given I have bonuses in the system
        | name             | rewardType | oddsLadderOffset | customer_id |
        | uniboost_bonus_1 | uniboost   | 3                | 918791      |
        | uniboost_bonus_2 | uniboost   | 3                | 918791      |
        | uniboost_bonus_3 | uniboost   | 3                | 918791      |
        | uniboost_bonus_4 | uniboost   | 3                | 918791      |
        | uniboost_bonus_5 | uniboost   | 3                | 918791      |
    Then the HttpStatusCode should be 201
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys       |
        | 918791      | uniboost_bonus_user |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name                                                                       | bonus_name |
        | uniboost_bonus_1,uniboost_bonus_2,uniboost_bonus_3,uniboost_bonus_4,uniboost_bonus_5 |            |

Scenario: GetEntitlement request returns all rewards linked to template
    Given I have promotions in the system
        | name             | rewardType | oddsLadderOffset |
        | uniboost_promo_1 | uniboost   | 3                |
        | uniboost_promo_2 | uniboost   | 3                |
        | uniboost_promo_3 | uniboost   | 3                |
        | uniboost_promo_4 | uniboost   | 3                |
        | uniboost_promo_5 | uniboost   | 3                |
    Then the HttpStatusCode should be 201
    When I submit CreateMapping request for following promotions:
        | template_key        | promotions                                                                           |
        | uniboost_promo_user | uniboost_promo_1,uniboost_promo_2,uniboost_promo_3,uniboost_promo_4,uniboost_promo_5 |
    Then the HttpStatusCode should be 200
    When I submit GetEntitlement request for following criteria:
        | customer_id | template_keys       |
        | 918791      | uniboost_promo_user |
    Then the HttpStatusCode should be 200
    And GetEntitlement response should include following rewards:
        | promotion_name                                                                       | bonus_name |
        | uniboost_promo_1,uniboost_promo_2,uniboost_promo_3,uniboost_promo_4,uniboost_promo_5 |            |


Scenario: Get entitlements should not return cancelled promotions
	When I create Bonus for following criteria:
		| customer_id | name |
		| 918737      | 123  |
	Then the HttpStatusCode should be 201
	When I submit CreatePromotion request for following promotions:
		| promotion_name | claim_limit |
		| new_uniboost   | 1           |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
		| template_key | promotions   |
		| new_user     | new_uniboost |
	Then the HttpStatusCode should be 200
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name | bonus_name |
		| new_uniboost   | 123        |
	When I submit CancelPromotion request for 'new_uniboost'
	Then the HttpStatusCode should be 200
	And the cache for template key 'new_user' is deleted
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should not include promotion 'new_uniboost'

Scenario: Get entitlements should return updated promotions
	When I create Bonus for following criteria:
		| customer_id | name |
		| 918737      | 123  |
	Then the HttpStatusCode should be 201
	When I submit CreatePromotion request for following promotions:
		| promotion_name | claim_limit |
		| new_uniboost   | 1           |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
		| template_key | promotions   |
		| new_user     | new_uniboost |
	Then the HttpStatusCode should be 200
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name | bonus_name |
		| new_uniboost   | 123        |
	When I UpdatePromotion for 'new_uniboost'
	Then the cache for template key 'new_user' is deleted
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name | bonus_name |
		| new_uniboost   | 123        |

Scenario: Get entitlements - with proper next refresh interval
	When I submit CreatePromotion request for following promotions:
		| promotion_name            | claim_limit | claim_interval |
		| new_uniboost_NextInterval | 1           | 0 0 0 ? * * *  |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
		| template_key | promotions                |
		| new_user     | new_uniboost_NextInterval |
	Then the HttpStatusCode should be 200
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And Rewards should have nextInterval field:
		| promotion_name            |
		| new_uniboost_NextInterval |

Scenario: Get entitlements should return new promotions mapped to a template
	When I create Bonus for following criteria:
		| customer_id | name |
		| 918737      | 123  |
	Then the HttpStatusCode should be 201
	When I submit CreatePromotion request for following promotions:
		| promotion_name | claim_limit |
		| new_uniboost   | 1           |
		| new_happy_hour | 1           |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
		| template_key | promotions   |
		| new_user     | new_uniboost |
	Then the HttpStatusCode should be 200
	And the cache for template key 'new_user' is deleted
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name | bonus_name |
		| new_uniboost   | 123        |
	When I submit CreateMapping request for following promotions:
		| template_key | promotions                  |
		| new_user     | new_uniboost,new_happy_hour |
	Then the HttpStatusCode should be 200
	And the cache for template key 'new_user' is deleted
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name              | bonus_name |
		| new_uniboost,new_happy_hour | 123        |

Scenario: Get entitlements 
	When I create Bonus for following criteria:
		| customer_id | name |
		| 918737      | 123  |
	Then the HttpStatusCode should be 201
	When I submit CreatePromotion request for following promotions:
		| promotion_name | claim_limit |
		| vip_uniboost   | 1           |
		| new_uniboost   | 1           |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
		| template_key | promotions   |
		| vip_user     | vip_uniboost |
		| new_user     | new_uniboost |
	Then the HttpStatusCode should be 200
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | new_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name | bonus_name |
		| new_uniboost   | 123        |
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys     |
		| 918737      | new_user,vip_user |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should include following rewards:
		| promotion_name            | bonus_name |
		| new_uniboost,vip_uniboost | 123        |

Scenario: Disabling a Promo template means entitlements for that template will not be returned
	Given I have promotions in the system
		| name   | rewardType |
		| oddsb1 | uniboost   |
	When I submit CreateMapping request for following promotions:
		| template_key | promotions |
		| vip_user     | oddsb1     |
	Then the HttpStatusCode should be 200
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | vip_user      |
	Then the HttpStatusCode should be 200
    And the entitlements for customer '918737' should contain:
        | name   | remainingClaimsPerInterval |
        | oddsb1 | 1                          |
	When I submit Delete request for template key 'vip_user'
	Then the HttpStatusCode should be 200
	And the cache for template key 'vip_user' is deleted
	When I submit GetEntitlement request for following criteria:
		| customer_id | template_keys |
		| 918737      | vip_user      |
	Then the HttpStatusCode should be 200
	And GetEntitlement response should not include promotion 'oddsb1'
