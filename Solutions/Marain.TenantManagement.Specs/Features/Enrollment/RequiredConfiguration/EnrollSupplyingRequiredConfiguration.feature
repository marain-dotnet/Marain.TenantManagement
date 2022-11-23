@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service Requiring V3 Configuration Supplying V3
    In order to allow a client to use a Marain service that requires V3 configuration
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC1D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC3D()' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | Service with    |
# | Litware +---------> 1 Configuration |
# |         |         | 0 Dependencies  |
# +---------+         +-----------------+
#
# Expected tenant tree:
#
# Root tenant
#  |
#  +-> Client Tenants
#  |     |
#  |     +-> Litware
#  |
#  +-> Service Tenants
#        |
#        +-> SvcC1D()
#
Scenario: Basic enrollment with configuration
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                            | Account Name | Container     |
    | TestServices:C1D():FooBarStore | blobaccount  | blobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D()'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'blobaccount' and container name 'blobcontainer'

    
# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 3 Configurations |
# |         |         | 0 Dependencies   |
# +---------+         +------------------+
#
# Expected tenant tree:
#
# Root tenant
#  |
#  +-> Client Tenants
#  |     |
#  |     +-> Litware
#  |
#  +-> Service Tenants
#        |
#        +-> SvcC3D()
#
Scenario: Basic enrollment with multiple configuration types
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                          | Account Name | Container     |
    | TestServices:C3D():QuuxStore | blobaccount  | blobcontainer |
	And the enrollment configuration called 'Config' contains the following Cosmos configuration items
	| Key                           | Account Uri   | Database Name | Container Name |
	| TestServices:C3D():SpongStore | cosmosaccount | db            | spongs         |
	And the enrollment configuration called 'Config' contains the following Table Storage configuration items
	| Key                       | Account Name | Table   |
	| TestServices:C3D():Wibble | tableaccount | wibbles |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC3D()'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC3D()' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C3D():QuuxStore' for the account 'blobaccount' and container name 'blobcontainer'
    And the tenant called 'Litware' should contain Cosmos configuration under the key 'TestServices:C3D():SpongStore' with database name 'db' and container name 'spongs'
    And the tenant called 'Litware' should contain table storage configuration under the key 'TestServices:C3D():Wibble' for the account 'tableaccount' and table name 'wibbles'
