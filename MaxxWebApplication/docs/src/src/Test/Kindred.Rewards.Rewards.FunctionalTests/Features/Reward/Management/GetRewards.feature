Feature: GetRewards

[GET] /rewards

@Jurisdiction
Scenario: Get Rewards returns ascending records sorted by Jurisdiction
    Given I have bonuses in the system
        | name      | rewardType  | customer_id               | jurisdiction |
        | sorting_1 | profitboost | customer_jurisdiction_asc | A            |
        | sorting_2 | profitboost | customer_jurisdiction_asc | B            |
        | sorting_3 | profitboost | customer_jurisdiction_asc | C            |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName    | sortOrder | customer_id               |
        | Jurisdiction | Ascending | customer_jurisdiction_asc |
    Then I expect the results in the following order for sort field 'jurisdiction'
        | name      | rewardType  | customer_id               | jurisdiction |
        | sorting_1 | ProfitBoost | customer_jurisdiction_asc | A            |
        | sorting_2 | ProfitBoost | customer_jurisdiction_asc | B            |
        | sorting_3 | ProfitBoost | customer_jurisdiction_asc | C            |
 
Scenario: Get Rewards returns descending records sorted by Jurisdiction
    Given I have bonuses in the system
        | name      | rewardType  | customer_id                | jurisdiction |
        | sorting_1 | profitboost | customer_jurisdiction_desc | A            |
        | sorting_2 | profitboost | customer_jurisdiction_desc | B            |
        | sorting_3 | profitboost | customer_jurisdiction_desc | C            |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName    | sortOrder  | customer_id                |
        | Jurisdiction | Descending | customer_jurisdiction_desc |
    Then I expect the results in the following order for sort field 'jurisdiction'
        | name      | rewardType  | customer_id                | jurisdiction |
        | sorting_3 | ProfitBoost | customer_jurisdiction_desc | C            |
        | sorting_2 | ProfitBoost | customer_jurisdiction_desc | B            |
        | sorting_1 | ProfitBoost | customer_jurisdiction_desc | A            |

@CountryCode
Scenario: Get Rewards returns ascending records sorted by CountryCode
    Given I have bonuses in the system
        | name      | rewardType  | customer_id               | countryCode |
        | sorting_1 | profitboost | customer_country_code_asc | AUS         |
        | sorting_2 | profitboost | customer_country_code_asc | BEL         |
        | sorting_3 | profitboost | customer_country_code_asc | CAN         |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName   | sortOrder | customer_id               |
        | CountryCode | Ascending | customer_country_code_asc |
    Then I expect the results in the following order for sort field 'countryCode'
        | name      | rewardType  | customer_id               | countryCode |
        | sorting_1 | ProfitBoost | customer_country_code_asc | AUS         |
        | sorting_2 | ProfitBoost | customer_country_code_asc | BEL         |
        | sorting_3 | ProfitBoost | customer_country_code_asc | CAN         |
 
Scenario: Get Rewards returns descending records sorted by CountryCode
    Given I have bonuses in the system
        | name      | rewardType  | customer_id                | countryCode |
        | sorting_1 | profitboost | customer_country_code_desc | AUS         |
        | sorting_2 | profitboost | customer_country_code_desc | BEL         |
        | sorting_3 | profitboost | customer_country_code_desc | CAN         |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName   | sortOrder  | customer_id                |
        | CountryCode | Descending | customer_country_code_desc |
    Then I expect the results in the following order for sort field 'countryCode'
        | name      | rewardType  | customer_id                | countryCode |
        | sorting_3 | ProfitBoost | customer_country_code_desc | CAN         |
        | sorting_2 | ProfitBoost | customer_country_code_desc | BEL         |
        | sorting_1 | ProfitBoost | customer_country_code_desc | AUS         |

@RewardType
Scenario: Get Rewards returns ascending records sorted by RewardType
    Given I have bonuses in the system
        | name      | rewardType     | customer_id              |
        | sorting_1 | profitboost    | customer_reward_type_asc |
        | sorting_2 | uniboostreload | customer_reward_type_asc |
        | sorting_3 | uniboost       | customer_reward_type_asc |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName  | sortOrder | customer_id              |
        | RewardType | Ascending | customer_reward_type_asc |
    Then I expect the results in the following order for sort field 'rewardType'
        | name      | rewardType     | customer_id              |
        | sorting_1 | Profitboost    | customer_reward_type_asc |
        | sorting_3 | Uniboost       | customer_reward_type_asc |
        | sorting_2 | UniboostReload | customer_reward_type_asc |
         
Scenario: Get Rewards returns descending records sorted by RewardType
    Given I have bonuses in the system
        | name      | rewardType     | customer_id               |
        | sorting_1 | profitboost    | customer_reward_type_desc |
        | sorting_2 | uniboostreload | customer_reward_type_desc |
        | sorting_3 | uniboost       | customer_reward_type_desc |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName  | sortOrder  | customer_id               |
        | RewardType | Descending | customer_reward_type_desc |
    Then I expect the results in the following order for sort field 'rewardType'
        | name      | rewardType     | customer_id               |
        | sorting_2 | UniboostReload | customer_reward_type_desc |
        | sorting_3 | Uniboost       | customer_reward_type_desc |
        | sorting_1 | Profitboost    | customer_reward_type_desc |

@CreatedBy
Scenario: Get Rewards returns ascending records sorted by CreatedBy
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | createdBy |
        | sorting_1 | profitboost    | customer_created_by_asc | Alpha     |
        | sorting_2 | uniboostreload | customer_created_by_asc | Bravo     |
        | sorting_3 | uniboost       | customer_created_by_asc | Charlie   |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder | customer_id             |
        | CreatedBy | Ascending | customer_created_by_asc |
    Then I expect the results in the following order for sort field 'createdBy'
        | name      | rewardType     | customer_id             | createdBy |
        | sorting_1 | ProfitBoost    | customer_created_by_asc | Alpha     |
        | sorting_2 | UniBoostReload | customer_created_by_asc | Bravo     |
        | sorting_3 | UniBoost       | customer_created_by_asc | Charlie   |
 
Scenario: Get Rewards returns descending records sorted by CreatedBy
    Given I have bonuses in the system
        | name      | rewardType     | customer_id              | createdBy |
        | sorting_1 | profitboost    | customer_created_by_desc | Alpha     |
        | sorting_2 | uniboostreload | customer_created_by_desc | Bravo     |
        | sorting_3 | uniboost       | customer_created_by_desc | Charlie   |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder  | customer_id              |
        | CreatedBy | Descending | customer_created_by_desc |
    Then I expect the results in the following order for sort field 'createdBy'
        | name      | rewardType     | customer_id              | createdBy |
        | sorting_3 | UniBoost       | customer_created_by_desc | Charlie   |
        | sorting_2 | UniBoostReload | customer_created_by_desc | Bravo     |
        | sorting_1 | ProfitBoost    | customer_created_by_desc | Alpha     |

@UpdatedBy
Scenario: Get Rewards returns ascending records sorted by UpdatedBy
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | updatedBy |
        | sorting_1 | profitboost    | customer_updated_by_asc | Alpha     |
        | sorting_2 | uniboostreload | customer_updated_by_asc | Bravo     |
        | sorting_3 | uniboost       | customer_updated_by_asc | Charlie   |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder | customer_id             |
        | UpdatedBy | Ascending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'updatedBy'
        | name      | rewardType     | customer_id             | updatedBy |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Alpha     |
        | sorting_2 | UniBoostReload | customer_updated_by_asc | Bravo     |
        | sorting_3 | UniBoost       | customer_updated_by_asc | Charlie   |
 
Scenario: Get Rewards returns descending records sorted by UpdatedBy
    Given I have bonuses in the system
        | name      | rewardType     | customer_id              | updatedBy |
        | sorting_1 | profitboost    | customer_updated_by_desc | Alpha     |
        | sorting_2 | uniboostreload | customer_updated_by_desc | Bravo     |
        | sorting_3 | uniboost       | customer_updated_by_desc | Charlie   |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder  | customer_id              |
        | UpdatedBy | Descending | customer_updated_by_desc |
    Then I expect the results in the following order for sort field 'updatedBy'
        | name      | rewardType     | customer_id              | updatedBy |
        | sorting_3 | UniBoost       | customer_updated_by_desc | Charlie   |
        | sorting_2 | UniBoostReload | customer_updated_by_desc | Bravo     |
        | sorting_1 | ProfitBoost    | customer_updated_by_desc | Alpha     |

@StartDateTime
Scenario: Get Rewards returns ascending records sorted by StartDateTime
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName     | sortOrder | customer_id             |
        | StartDateTime | Ascending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'startDateTime'
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | UniBoostReload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | UniBoost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
 
Scenario: Get Rewards returns descending records sorted by StartDateTime
    Given I have bonuses in the system
        | name      | rewardType     | customer_id              | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_desc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_desc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_desc | 2020-08-18T07:22:16.0000000Z |
    Then the HttpStatusCode should be 201
    When I submit GetRewards request with the following criteria
        | fieldName     | sortOrder  | customer_id              |
        | StartDateTime | Descending | customer_updated_by_desc |
    Then I expect the results in the following order for sort field 'startDateTime'
        | name      | rewardType     | customer_id              | startDateTime                |
        | sorting_3 | UniBoost       | customer_updated_by_desc | 2020-08-18T07:22:16.0000000Z |
        | sorting_2 | UniBoostReload | customer_updated_by_desc | 2019-08-18T07:22:16.0000000Z |
        | sorting_1 | ProfitBoost    | customer_updated_by_desc | 2018-08-18T07:22:16.0000000Z |

Scenario: Get Rewards returns ascending records sorted by Status - one expired
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    And I submit UpdateBonus request for 'sorting_3' to expire
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder | customer_id             |
        | Status    | Ascending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status  |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active  |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Active  |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Expired |
 
Scenario: Get Rewards returns ascending records sorted by Status - all active - should be sorted by start date time
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder | customer_id             |
        | Status    | Ascending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Active |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Active |
 
Scenario: Get Rewards returns ascending records sorted by Status - all active - should be sorted by start date time then by name
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder | customer_id             |
        | Status    | Ascending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Active |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Active |

Scenario: Get Rewards returns ascending records sorted by Status - one active, one expired and one cancelled
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    And I submit UpdateBonus request for 'sorting_3' to expire
    When I submit CancelBonus request for 'sorting_2'
    And I submit GetRewards request with the following criteria
        | fieldName | sortOrder | customer_id             |
        | Status    | Ascending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status    |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active    |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Cancelled |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Expired   |

Scenario: Get Rewards returns descending records sorted by Status - one expired
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    And I submit UpdateBonus request for 'sorting_3' to expire
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder  | customer_id             |
        | Status    | descending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status  |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Expired |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active  |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Active  |
 
Scenario: Get Rewards returns descending records sorted by Status - all active - should be sorted by start date time
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder  | customer_id             |
        | Status    | descending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Active |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Active |
 
Scenario: Get Rewards returns descending records sorted by Status - all active - should be sorted by start date time then by name
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
    When I submit GetRewards request with the following criteria
        | fieldName | sortOrder  | customer_id             |
        | Status    | descending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Active |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Active |

Scenario: Get Rewards returns descending records sorted by Status - one active, one expired and one cancelled
    Given I have bonuses in the system
        | name      | rewardType     | customer_id             | startDateTime                |
        | sorting_1 | profitboost    | customer_updated_by_asc | 2018-08-18T07:22:16.0000000Z |
        | sorting_2 | uniboostreload | customer_updated_by_asc | 2019-08-18T07:22:16.0000000Z |
        | sorting_3 | uniboost       | customer_updated_by_asc | 2020-08-18T07:22:16.0000000Z |
    And I submit UpdateBonus request for 'sorting_3' to expire
    When I submit CancelBonus request for 'sorting_2'
    And I submit GetRewards request with the following criteria
        | fieldName | sortOrder  | customer_id             |
        | Status    | descending | customer_updated_by_asc |
    Then I expect the results in the following order for sort field 'Status'
        | name      | rewardType     | customer_id             | status    |
        | sorting_3 | Uniboost       | customer_updated_by_asc | Expired   |
        | sorting_2 | UniboostReload | customer_updated_by_asc | Cancelled |
        | sorting_1 | ProfitBoost    | customer_updated_by_asc | Active    |

Scenario: Get all bonuses by UpdateDateTime
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
    When I UpdateBonus for 'uniboost_5_pct_bonus'
    Then I capture the UpdateDateFrom time
    When I UpdateBonus for 'uniboost_6_pct_bonus'
    When I UpdateBonus for 'uniboost_7_pct_bonus'
    When I UpdateBonus for 'uniboost_8_pct_bonus'
    Then I capture the UpdateDateTo time
    When I UpdateBonus for 'uniboost_9_pct_bonus'
    When I UpdateBonus for 'uniboost_4_pct_bonus'
	
	# Get rewards within the filter UpdateTime window
    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive |
        | true             | true           | true          |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should not return bonus 'uniboost_5_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_6_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_7_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_8_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_9_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_4_pct_bonus'

    
@bonus
Scenario: Get all bonuses by filter
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

    Given I submit UpdateBonus request for 'uniboost_4_pct_bonus' to expire
	
	# Get expired reward
    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id |
        | true             | true           | true          | 123         |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should return bonus 'uniboost_4_pct_bonus'

	# Get active reward
    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id |
        | false            | false          | true          | 123         |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should not return bonus 'uniboost_4_pct_bonus'

@bonus
Scenario: Get all bonuses by filter - filter by country
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

    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id | country |
        | true             | true           | true          | 123         | uSa     |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should return bonus 'uniboost_8_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_9_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_4_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_3_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_7_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_6_pct_bonus'
    And the GetBonuses response should not return bonus 'uniboost_5_pct_bonus'

@bonus
Scenario: Get all bonuses by filter - IncludeActive set to true returns scheduled and active
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

    When I submit GetBonuses request for following criteria
        | includeCancelled | includeExpired | includeActive | customer_id |
        | false            | false          | true          | 123         |
    Then the HttpStatusCode should be 200
    And the GetBonuses response should return bonus 'uniboost_6_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_7_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_8_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_9_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_4_pct_bonus'
    And the GetBonuses response should return bonus 'uniboost_3_pct_bonus'

@bonus
Scenario: Get all bonuses by customer ID
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

    When I submit GetBonuses request with customer id '321'
    Then the HttpStatusCode should be 200
    And GetBonuses response should return all bonuses with name 'uniboost_5_pct_bonus'


@promotion
Scenario: Get promotion with templates
    Given I have promotions in the system
        | name           | rewardType | expiryDaysFromNow                       | startDaysFromNow |
        | uniboost_5_pct | uniboost   |                                         |                  |
        | uniboost_6_pct | uniboost   |                                         |                  |
        | uniboost_7_pct | uniboost   |                                         | 1                |
        | uniboost_8_pct | uniboost   |                                         |                  |
        | uniboost_9_pct | uniboost   | time-in-next-daylight-saving-time-shift | 1                |
        | uniboost_4_pct | uniboost   |                                         | 1                |
    Given I have promotions templates in the system
        | key      | comments       |
        | freebet  | freebet promo  |
        | uniboost | uniboost promo |
	
    When I submit CreateMapping request for following promotions:
        | template_key | promotions                    |
        | freebet      | uniboost_5_pct                |
        | uniboost     | uniboost_5_pct,uniboost_6_pct |
    Then the HttpStatusCode should be 200

    When I submit GetPromotion request for 'uniboost_5_pct'
    Then the HttpStatusCode should be 200
    And the GetPromotion response should return promotion 'uniboost_5_pct'
    And the GetPromotion response should return promotion templates 'freebet,uniboost'

    When I submit GetPromotion request for 'uniboost_6_pct'
    Then the HttpStatusCode should be 200
    And the GetPromotion response should return promotion 'uniboost_6_pct'
    And the GetPromotion response should return promotion templates 'uniboost'

@promotion
Scenario: Get promotions with templates
    Given I have promotions in the system
        | name           | rewardType | expiryDaysFromNow                       | startDaysFromNow |
        | uniboost_5_pct | uniboost   |                                         |                  |
        | uniboost_6_pct | uniboost   |                                         |                  |
        | uniboost_7_pct | uniboost   |                                         | 1                |
        | uniboost_8_pct | uniboost   |                                         |                  |
        | uniboost_9_pct | uniboost   | time-in-next-daylight-saving-time-shift | 1                |
        | uniboost_4_pct | uniboost   |                                         | 1                |
    Given I have promotions templates in the system
        | key      | comments       |
        | freebet  | freebet promo  |
        | uniboost | uniboost promo |
	
    When I submit CreateMapping request for following promotions:
        | template_key | promotions     |
        | freebet      | uniboost_5_pct |
        | uniboost     | uniboost_5_pct |
    Then the HttpStatusCode should be 200

    When I submit GetPromotions request with name 'uniboost_5_pct'
    Then the HttpStatusCode should be 200
    And GetPromotions response should return all promotions with name 'uniboost_5_pct'
    And the GetPromotions response should return promotion templates 'freebet,uniboost' for promotion 'uniboost_5_pct'
