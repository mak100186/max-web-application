@Acceptance
@promo-templates
Feature: RewardTemplate
	In order to group multiple promotions to a single category. As a business user,	I want to create a reward template and map multiple rewards (aka promotions)
    to it so that customers under the template can be linked to a reward template

Background:
	Given I have promotions templates in the system
		| key		| comments 		|
		| freebet   | freebet promo   |
		| uniboost  | uniboost promo |

@promo-template
Scenario: Get promotion template by key
	When I submit Get request for template key 'freebet' for ActiveState is 'null'
	Then the HttpStatusCode should be 200
	And Get response should return template for template key 'freebet'

@promo-template
Scenario: Get all promotion templates
	When I submit GetAll request with following filters:
		| includeDisabled |
		| true            |
	Then the HttpStatusCode should be 200
	And GetAll response should return templates

@promo-template
Scenario: Get all promotion templates must include default templates
	When I submit GetAll request with following filters:
	| includeDisabled |
	| true            |
	Then the HttpStatusCode should be 200
	And GetAll response should return templates
	And GetAll response should include the following templates:
	| template				 |
	| UB_NEW				 |
	| UB_STANDARD			 |
	| UB_VALUED_SMALL		 |
	| UB_VALUED_MEDIUM		 |
	| UB_VALUED_LARGE		 |
	| UB_VIP_SMALL			 |
	| UB_VIP_MEDIUM			 |
	| UB_VIP_LARGE			 |
	| NO_REWARDS			 |
	| UB_NO_REWARDS_FALLBACK |

@promo-template
Scenario: Duplicate promotion template request should throw error
	When I submit Create request with template key 'freebet'
	Then the HttpStatusCode should be 400
	And Response should contain error 'Promotion template exist with template key QhePSHBDc3hQUQv57OuFfreebet'

@promo-template
Scenario: Get promotion template throws 404 if not found
	When I submit Get request for template key 'FREEBET1' for ActiveState is 'null'
	Then the HttpStatusCode should be 404

@promo-template
Scenario: Disable promotion template
	When I submit Delete request for template key 'freebet'
	Then the HttpStatusCode should be 200
	When I submit Get request for template key 'freebet' for ActiveState is 'mull'
	Then the HttpStatusCode should be 200
	And the template key 'freebet' should be disabled
	When I submit GetAll request with following filters:
		| includeDisabled |
		| false           |
	Then the HttpStatusCode should be 200
	And GetAll response should not return template key 'freebet'

@promo-template
Scenario: Map promotion template to promotions throws 400 if reward not found because Rn is invalid
	When I submit CreateMapping request for following rewards that do not exist:
		| template_key | promotions | reward_rn      |
		| uniboost     | blah       | someInvalidRn  |
	Then the HttpStatusCode should be 400
	And Response should contain error 'Could not parse the provided RewardRns. Supported values are the Rn identifier or the full Rn. Provided Rns: someInvalidRn'

@promo-template
Scenario: Map promotion template to promotions throws 404 if reward not found
	When I submit CreateMapping request for following rewards that do not exist:
		| template_key | promotions | reward_rn                            |
		| uniboost     | blah       | 20d74244-efde-49f9-bb82-e701b3a5b9f0 |
	Then the HttpStatusCode should be 404
	And Response should contain error 'Could not find promotions with reward keys 20d74244-efde-49f9-bb82-e701b3a5b9f0'

@promo-template
Scenario: Map promotion template to promotions
	When I submit CreatePromotion request for following promotions:
		| promotion_name      | claim_limit |
		| vip_odds_boost      | 1           |
		| standard_odds_boost | 1           |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
		| template_key	| promotions						 |
		| uniboost      | vip_odds_boost,standard_odds_boost |
	Then the HttpStatusCode should be 200
	When I submit Get request for template key 'uniboost' for ActiveState is 'null'
	Then Get response should return template 'uniboost' with mapped promotions 'vip_odds_boost,standard_odds_boost'


@promo-template
Scenario: Cancelling Mapped Promotion Sould Not Unmap other promotions linked to Promot Template
	Given I have promotions in the system
	| name   | rewardType |
	| oddsb1 | uniboost   |
	| oddsb2 | uniboost   |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
	| template_key | promotions    |
	| uniboost     | oddsb1,oddsb2 |
	Then the HttpStatusCode should be 200
	When I submit Get request for template key 'uniboost' for ActiveState is 'null'
	Then Get response should return template 'uniboost' with mapped promotions 'oddsb1,oddsb2'
	When I submit CancelPromotion request for 'oddsb1'
	Then the HttpStatusCode should be 200
	When I submit Get request for template key 'uniboost' for ActiveState is 'null'
	Then Get response should return template 'uniboost' with mapped promotions 'oddsb2'

@promo-template
Scenario: Get Promotions When Requesting Active Promotions Should only return active promotions for template
	Given I have promotions in the system
	| name   | rewardType | startDaysFromNow | expiryDaysFromNow |
	| oddsb1 | uniboost   | -1               | 1                 |
	| oddsb2 | uniboost   | 5                | 10                |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
	| template_key | promotions    |
	| uniboost    | oddsb1,oddsb2 |
	Then the HttpStatusCode should be 200
	When I submit Get request for template key 'uniboost' for ActiveState is 'true'
	Then Get response should return template 'uniboost' with mapped promotions 'oddsb1'

@promo-template
Scenario: Get Promotions When Requesting NonActive Promotions Should only return non active promotions for template
	Given I have promotions in the system
	| name   | rewardType | startDaysFromNow | expiryDaysFromNow |
	| oddsb1 | uniboost   | -1               | 1                 |
	| oddsb2 | uniboost   | 5                | 10                |
	Then the HttpStatusCode should be 201
	When I submit CreateMapping request for following promotions:
	| template_key | promotions    |
	| uniboost     | oddsb1,oddsb2 |
	Then the HttpStatusCode should be 200
	When I submit Get request for template key 'uniboost' for ActiveState is 'false'
	Then Get response should return template 'uniboost' with mapped promotions 'oddsb2'

@promo-template
@ignore
Scenario: Authenticated user creates a promotion template Should return createdBy
Given I submit CreatePromotion request by user 'crm-user-name'
  		| template_key      | comments | title     |
  		| uniboost-blah     | blah     | blah-blah |
        Then the HttpStatusCode should be 201
        When I submit Get request for template key 'uniboost-blah' for ActiveState is 'null'
	    Then Get response should return expected createdBy field with value 'crm-user-name'


@promo-template
@ignore
Scenario: Authenticated user maps a promotion template Should return updatedBy
Given I submit MapPromotion request by user 'crm-user-name-2'
	    | template_key      | promotions    |
	    | uniboost-blah     | oddsb1,oddsb2 |
        Then the HttpStatusCode should be 201
        When I submit Get request for template key 'uniboost-blah' for ActiveState is 'null'
	    Then Get response should return expected updatedBy with value 'crm-user-name-2'
