@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service Requiring V3 Configuration Supplying V3
    In order to allow a client to use a Marain service that requires V3 configuration
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC1D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D()'
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

# TODO: table storage and cosmos
# See Enrollment.feature