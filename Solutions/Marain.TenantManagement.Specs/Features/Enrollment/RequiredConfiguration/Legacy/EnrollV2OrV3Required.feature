@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service Requiring V2 or V3 Configuration
    In order to allow a client to use a Marain service that supports either V2 or V3 configuration
    As an administrator
    I want to enroll a tenant to use that service using either V2 or V3 configuration

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'LegacyV2.SvcC3V3orV2D()' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

Scenario: Basic enrollment with multiple V3 configuration types
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                                                  | Account Name | Container     |
    | V3WithV2Migration:BlobContainerConfiguration:myblobs | blobaccount  | blobcontainer |
	And the enrollment configuration called 'Config' contains the following Cosmos configuration items
	| Key                                                        | Account Uri   | Database Name | Container Name |
	| V3WithV2Migration:CosmosContainerConfiguration:mycontainer | cosmosaccount | db            | spongs         |
	And the enrollment configuration called 'Config' contains the following Table Storage configuration items
	| Key                                          | Account Name | Table   |
	| V3WithV2Migration:TableConfiguration:mytable | tableaccount | wibbles |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC3V3orV2D()'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC3V3orV2D()' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'V3WithV2Migration:BlobContainerConfiguration:myblobs' for the account 'blobaccount' and container name 'blobcontainer'
    And the tenant called 'Litware' should contain Cosmos configuration under the key 'V3WithV2Migration:CosmosContainerConfiguration:mycontainer' with database name 'db' and container name 'spongs'
    And the tenant called 'Litware' should contain table storage configuration under the key 'V3WithV2Migration:TableConfiguration:mytable' for the account 'tableaccount' and table name 'wibbles'

Scenario: Basic enrollment with multiple V2 configuration types
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following legacy V2 Blob Storage configuration items
    | Key                                                  | Account Name | Container     |
    | V3WithV2Migration:BlobContainerConfiguration:myblobs | blobaccount  | blobcontainer |
	And the enrollment configuration called 'Config' contains the following legacy V2 Cosmos configuration items
	| Key                                                        | Account Uri   | Database Name | Container Name |
	| V3WithV2Migration:CosmosContainerConfiguration:mycontainer | cosmosaccount | db            | spongs         |
	And the enrollment configuration called 'Config' contains the following legacy V2 Table Storage configuration items
	| Key                                          | Account Name | Table   |
	| V3WithV2Migration:TableConfiguration:mytable | tableaccount | wibbles |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC3V3orV2D()'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC3V3orV2D()' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'V3WithV2Migration:BlobContainerConfiguration:myblobs' for the account 'blobaccount' and container name 'blobcontainer'
    And the tenant called 'Litware' should contain Cosmos configuration under the key 'V3WithV2Migration:CosmosContainerConfiguration:mycontainer' with database name 'db' and container name 'spongs'
    And the tenant called 'Litware' should contain table storage configuration under the key 'V3WithV2Migration:TableConfiguration:mytable' for the account 'tableaccount' and table name 'wibbles'
