@Acceptance
Feature: UniboostReloadPromotion
	In order to keep customers betting with us
	As a business
	I want to offer them reload on UniboostReload promotion until they win

@uniboost-reload
Scenario: Create a UniboostReload promotion - red path
	When I submit CreatePromotion request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | null              | true         | 1         | 1                    |
	Then the HttpStatusCode should be 400
	And Response should contain error 'Reload configuration is not required if ClaimsPerInterval is null'

	When I submit CreatePromotion request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | 1                 | true         | 0         | 1                    |
	| UniboostReload | 1                 | true         | null      | 1                    |
	Then the HttpStatusCode should be 400
	And Response should contain error 'MaxReload should be null or greater than 0'

	When I submit CreatePromotion request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | 1                 | true         | 2         | 0                    |
	Then the HttpStatusCode should be 400
	And Response should contain error 'StopOnMinimumWinBets should be greater than 0'

@uniboost-reload
Scenario: Create a UniboostReload promotion
	When I submit CreatePromotion request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | 1                 | true         | 1         | 1                    |
	Then the HttpStatusCode should be 201
	And UniboostReload promotion should have been created
