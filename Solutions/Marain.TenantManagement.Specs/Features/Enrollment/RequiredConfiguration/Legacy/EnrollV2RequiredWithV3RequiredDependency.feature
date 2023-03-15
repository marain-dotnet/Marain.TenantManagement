@perScenarioContainer
@useInMemoryTenantProvider

Feature: EnrollV2RequiredWithV3RequiredDependency
    In order to allow a client to use a Marain service requiring V2 configuration that depends on another Marain service that requires V3 configuration
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC1D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'LegacyV2.SvcC1V2D(C1D())' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+      +-----------------+
# |         |      | SvcC1V2D(C1D()) |
# | Litware +------> 1 Configuration +-----+
# |         |      | 1 Dependency    |     |
# +---------+      +-----------------+     |
#                                          |
#                                          |
#                                 +--------v--------+
#                                 | SvcC1D()        |
#                                 | 1 Configuration |
#                                 | 0 Dependencies  |
#                                 +-----------------+
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
#        +-> SvcC1V2D(C1D())
#        |     |
#        |     +-> SvcC1V2D(C1D())\Litware
#        |
#        +-> SvcC1D()
#
Scenario: Enroll
    Given I have enrollment configuration called 'ConfigSvcC1V2D(C1D())'
    And I have enrollment configuration called 'ConfigSvcC1D()'
    And the 'ConfigSvcC1V2D(C1D())' enrollment has a dependency on the service tenant called 'SvcC1D()' using configuration 'ConfigSvcC1D()'
    And the enrollment configuration called 'ConfigSvcC1V2D(C1D())' contains the following legacy V2 Blob Storage configuration items
    | Key         | Account Name | Container      |
    | mycontainer | blobaccount1 | blobcontainer1 |
    And the enrollment configuration called 'ConfigSvcC1D()' contains the following Blob Storage configuration items
    | Key                            | Account Name | Container      |
    | TestServices:C1D():FooBarStore | blobaccount2 | blobcontainer2 |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1V2D(C1D())' to enroll the tenant called 'Litware' in the service called 'SvcC1V2D(C1D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1V2D(C1D())' added to its enrollments
    And the tenant called 'Litware' should contain legacy V2 blob storage configuration under the key 'StorageConfiguration__mycontainer' for the account 'blobaccount1' and container name 'blobcontainer1'
    And a new child tenant called 'SvcC1V2D(C1D())\Litware' of the service tenant called 'SvcC1V2D(C1D())' has been created
    And the tenant called 'SvcC1V2D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC1V2D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'blobaccount2' and container name 'blobcontainer2'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1V2D(C1D())\Litware' set as the delegated tenant for the service called 'SvcC1V2D(C1D())'
