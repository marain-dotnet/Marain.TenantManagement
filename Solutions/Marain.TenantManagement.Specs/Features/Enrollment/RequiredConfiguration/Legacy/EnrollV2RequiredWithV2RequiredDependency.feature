@perScenarioContainer
@useInMemoryTenantProvider

Feature: EnrollV2RequiredWithV2RequiredDependency
    In order to allow a client to use a Marain service requiring V2 configuration that depends on another Marain service that requires V2 configuration
    As an administrator
    I want to enroll a tenant to use that service using either V2 or V3 configuration for the dependency

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'LegacyV2.SvcC1V2D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'LegacyV2.SvcC1V2D(C1V2D())' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+      +-------------------+
# |         |      | SvcC1V2D(C1V2D()) |
# | Litware +------> 1 Configuration   +-----+
# |         |      | 1 Dependency      |     |
# +---------+      +-------------------+     |
#                                            |
#                                            |
#                                   +--------v--------+
#                                   | SvcC1V2D()      |
#                                   | 1 Configuration |
#                                   | 0 Dependencies  |
#                                   +-----------------+
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
#        +-> SvcC1V2D(C1V2D())
#        |     |
#        |     +-> SvcC1V2D(C1V2D())\Litware
#        |
#        +-> SvcC1V2D()
#

Scenario: Enroll
    Given I have enrollment configuration called 'ConfigSvcC1V2D(C1V2D())'
    And I have enrollment configuration called 'ConfigSvcC1V2D()'
    And the 'ConfigSvcC1V2D(C1V2D())' enrollment has a dependency on the service tenant called 'SvcC1V2D()' using configuration 'ConfigSvcC1V2D()'
    And the enrollment configuration called 'ConfigSvcC1V2D(C1V2D())' contains the following legacy V2 Blob Storage configuration items
    | Key       | Account Name | Container      |
    | container | blobaccount1 | blobcontainer1 |
    And the enrollment configuration called 'ConfigSvcC1V2D()' contains the following legacy V2 Blob Storage configuration items
    | Key       | Account Name | Container      |
    | container | blobaccount2 | blobcontainer2 |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1V2D(C1V2D())' to enroll the tenant called 'Litware' in the service called 'SvcC1V2D(C1V2D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1V2D(C1V2D())' added to its enrollments
    And the tenant called 'Litware' should contain legacy V2 blob storage configuration under the key 'StorageConfiguration__mycontainer' for the account 'blobaccount1' and container name 'blobcontainer1'
    And a new child tenant called 'SvcC1V2D(C1V2D())\Litware' of the service tenant called 'SvcC1V2D(C1V2D())' has been created
    And the tenant called 'SvcC1V2D(C1V2D())\Litware' should have the id of the tenant called 'SvcC1V2D()' added to its enrollments
    And the tenant called 'SvcC1V2D(C1V2D())\Litware' should contain legacy V2 blob storage configuration under the key 'StorageConfiguration__mycontainer' for the account 'blobaccount2' and container name 'blobcontainer2'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1V2D(C1V2D())\Litware' set as the delegated tenant for the service called 'SvcC1V2D(C1V2D())'
