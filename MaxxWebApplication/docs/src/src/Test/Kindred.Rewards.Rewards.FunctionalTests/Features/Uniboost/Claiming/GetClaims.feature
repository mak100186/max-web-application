Feature: GetClaims

[GET] /claims

Background: 
	Given I have claims in the system
		| customer_id | name    | instance_id | bet_client_ref | sequence_no | bet_outcome_status | CurrencyCode |
		| 92819278    | claim_1 | 1           | -81            | 999999999   |    Losing          | AUD          |  
		| 92819279    | claim_2 | 2           | -82            | 999999998   |                    |              |
		| 92819279    | claim_3 | 3           | -83            | 999999997   |                    | AUD          |

Scenario: Get claims by filter - No filter
	When I submit GetClaims request for empty criteria
	Then the HttpStatusCode should be 200
	
Scenario: Get claims by filter - Default ascending order of cliams
	When I submit GetClaims request for criteria
		| customer_id |
		| 92819279    |
	Then the HttpStatusCode should be 200
	And the claims are in following order of their update times
		| sort_order |
		| ascending  |
	And Claims should have correct CurrencyCode values
	
Scenario: Get claims by filter - Descending order of cliams
	When I submit GetClaims request for criteria
		| customer_id | is_descending |
		| 92819279    | true          |
	Then the HttpStatusCode should be 200
	And the claims are in following order of their update times
		| sort_order |
		| descending |
	
Scenario: Get claims by filter - filter by UpdateDateUTC. Also testing that the update time of '2019-10-06 02:03:05.119516', which doesn't exist in the AEST timezone, doesn't cause any error.
	When I update the UpdateTime of a claim for customerId '92819278' to '2019-10-06 02:03:05.119516'
	And I submit GetClaims request for criteria
		| update_date_time_utc_from   | update_date_time_utc_to     |
		| 2019-10-06 01:00:05.119516Z | 2019-10-06 04:00:05.119516Z |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |
	
Scenario: Get claims by filter - Filter by customer Id
	When I submit GetClaims request for criteria
		| customer_id |
		| 92819278    |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by instance Id
	When I submit GetClaims request for criteria
		| instance_id | row_count |
		| 1           | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by reward key
	When I submit a GetClaims request filtering by reward rn for claim: 'claim_1'
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |
	And GetClaims response should return betoutcome status
		| name      | bet_outcome_status |
		| claim_1   | Losing             |

Scenario: Get claims by filter - Filter by reward key for betoutcome
	When I submit a GetClaims request filtering by reward rn for claim: 'claim_2'
	Then the HttpStatusCode should be 200
	And GetClaims response should return betoutcome status
		| name      | bet_outcome_status |
		| claim_2   |                    |

Scenario: Get claims by filter - Filter by promo id
	When I submit GetClaims request for criteria
		| name    | row_count |
		| claim_1 | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by bet_client_ref
	When I submit GetClaims request for criteria
		| bet_client_ref | row_count |
		| -81            | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by sequence_no
	When I submit GetClaims request for criteria
		| sequence_no | row_count |
		| 999999999   | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by reward type
	When I submit GetClaims request for criteria
		| customer_id | reward_type | row_count |
		| 92819278    | Uniboost    | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by bet_outcome_status
	When I submit GetClaims request for criteria
		| customer_id | bet_outcome_status | row_count |
		| 92819278    | Losing             | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

Scenario: Get claims by filter - Filter by claim_status
	When I submit GetClaims request for criteria
		| customer_id | claim_status | row_count |
		| 92819278    | Claimed      | 1         |
	Then the HttpStatusCode should be 200
	And GetClaims response should return result
		| customer_id | row_count |
		| 92819278    | 1         |

