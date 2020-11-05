@perScenarioContainer
@useInMemoryTenantProvider

Feature: Add configuration
	In order to allow a client to be flexible with how they use the tenancy service
	As an administrator
	I want to add arbitrary configuration for that client

Background:
	Given the tenancy provider has been initialised for use with Marain
	And I have used the tenant management service to create a new client tenant called 'Contoso'

Scenario: Add blob storage configuration
	Given I have configuration called 'FooBar config'
	And the configuration called 'FooBar config' contains the following Blob Storage configuration items
	| Key         | Account Name | Container     |
	| fooBarStore | blobaccount  | blobcontainer |
	When I use the tenant management service with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
	Then the tenant called 'Contoso' should contain blob storage configuration for a blob storage container definition with container name 'foobar'

Scenario: Add table storage configuration
	Given I have configuration called 'FooBar config'
	And the configuration called 'FooBar config' contains the following Table Storage configuration items
	| Key         | Account Name | Table   |
	| fooBarStore | tableaccount | fbtable |
	When I use the tenant management service with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
	Then the tenant called 'Contoso' should contain table storage configuration for a table storage table definition with table name 'fbtable'

Scenario: Add cosmos configuration
	Given I have configuration called 'FooBar config'
	And the configuration called 'FooBar config' contains the following Cosmos configuration items
	| Key         | Account Uri   | Database Name | Container Name |
	| fooBarStore | cosmosaccount | db            | container      |
	When I use the tenant management service with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
	Then the tenant called 'Contoso' should contain Cosmos configuration for a Cosmos container definition with database name 'db' and container name 'container'