﻿@perScenarioContainer
@useInMemoryTenantProvider
@useChildObjects

Feature: Add or update configuration
	In order to allow a client to be flexible with how they use the tenancy service
	As an administrator
	I want to add/update arbitrary storage configuration for that client

Background:
	Given the tenancy provider has been initialised for use with Marain
	And I have used the tenant store to create a new client tenant called 'Contoso'

Scenario: Add blob storage configuration
	Given I have configuration called 'FooBar config'
	And the configuration called 'FooBar config' contains the following Blob Storage configuration items
	| Definition - Container | Configuration - Account Name | Configuration - Container |
	| foo                    | blobaccount                  | blobcontainer             |
	When I use the tenant store with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
	Then the tenant called 'Contoso' should contain blob storage configuration for a blob storage container definition with container name 'foo'

Scenario: Add table storage configuration
	Given I have configuration called 'FooBar config'
	And the configuration called 'FooBar config' contains the following Table Storage configuration items
	| Definition - Table | Configuration - Account Name | Configuration - Table |
	| foo                | tableaccount                 | fbtable               |
	When I use the tenant store with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
	Then the tenant called 'Contoso' should contain table storage configuration for a table storage table definition with table name 'foo'

Scenario: Add cosmos configuration
	Given I have configuration called 'FooBar config'
	And the configuration called 'FooBar config' contains the following Cosmos configuration items
	| Definition - Database | Definition - Container | Configuration - Account Uri | Configuration - Database | Configuration - Container |
	| foo                   | bar                    | cosmosaccount               | db                       | container                 |
	When I use the tenant store with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
	Then the tenant called 'Contoso' should contain Cosmos configuration for a Cosmos container definition with database name 'foo' and container name 'bar'