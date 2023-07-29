@Acceptance
Feature: UniboostReloadBonus
	In order to keep customers betting with us
	As a business
	I want to offer them reload on UniboostReload bonus until they win

@uniboost-reload
Scenario: Create a UniboostReload bonus - red path
	When I submit CreateBonus request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | null              | true         | 1         | 1                    |
	Then the HttpStatusCode should be 400
	And Response should contain error 'Reload configuration is not required if ClaimsPerInterval is null'

	When I submit CreateBonus request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | 1                 | true         | 0         | 1                    |
	| UniboostReload | 1                 | true         | null      | 1                    |
	Then the HttpStatusCode should be 400
	And Response should contain error 'MaxReload should be null or greater than 0'

	When I submit CreateBonus request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | 1                 | true         | 2         | 0                    |
	Then the HttpStatusCode should be 400
	And Response should contain error 'StopOnMinimumWinBets should be greater than 0'

@uniboost-reload
Scenario: Create a UniboostReload bonus
	When I submit CreateBonus request for UniboostReload with the following reload options
	| rewardType     | claimsPerInterval | enableReload | maxReload | stopOnMinimumWinBets |
	| UniboostReload | 1                 | true         | 1         | 1                    |
	Then the HttpStatusCode should be 201
	And UniboostReload bonus should have been created
