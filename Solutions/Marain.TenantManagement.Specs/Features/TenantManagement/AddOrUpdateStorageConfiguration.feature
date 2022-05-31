@perScenarioContainer
@useInMemoryTenantProvider
@useChildObjects

Feature: Add or update configuration
    In order to allow a client to be flexible with how they use the tenancy service
    As an administrator
    I want to add/update arbitrary storage configuration for that client

# TODO: the split between configuration and enrollment configuration is a bit confusing. We seem to be
# duplicating some things. Should enrollment configuration be recast in terms of wrappers around configuration types?

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have used the tenant store to create a new client tenant called 'Contoso'

Scenario: Add blob storage configuration
    Given I have configuration called 'FooBar config'
    And the configuration called 'FooBar config' contains the following Blob Storage configuration items
    | ConfigurationKey | Configuration - Account Name | Configuration - Container |
    | foo              | blobaccount                  | blobcontainer             |
    When I use the tenant store with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
    Then the tenant called 'Contoso' should contain blob storage configuration under the key 'foo' for the account 'blobaccount' and container name 'blobcontainer'

Scenario: Add table storage configuration
    Given I have configuration called 'FooBar config'
    And the configuration called 'FooBar config' contains the following Table Storage configuration items
    | ConfigurationKey | Configuration - Account Name | Configuration - Table |
    | foo              | tableaccount                 | fbtable               |
    When I use the tenant store with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
    Then the tenant called 'Contoso' should contain table storage configuration under the key 'foo' for the account 'tableaccount' and table name 'fbtable'

Scenario: Add cosmos configuration
    Given I have configuration called 'FooBar config'
    And the configuration called 'FooBar config' contains the following Cosmos configuration items
    | ConfigurationKey | Configuration - Account Uri | Configuration - Database | Configuration - Container |
    | foo              | cosmosaccount               | db                       | container                 |
    When I use the tenant store with the configuration called 'FooBar config' to add config for the tenant called 'Contoso'
    Then the tenant called 'Contoso' should contain Cosmos configuration under the key 'foo' with database name 'db' and container name 'container'