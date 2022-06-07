@perScenarioContainer
@useInMemoryTenantProvider

Feature: EnrollV3RequiredWithV2OrV3RequiredDependency
    In order to allow a client to use a Marain service requiring V3 configuration that depends on another Marain service that requires either V2 or V3 configuration
    As an administrator
    I want to enroll a tenant to use that service using either V2 or V3 configuration for the dependency

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'LegacyV2.SvcC1V3orV2D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'LegacyV2.SvcC1D(C1V3orV2D())' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+      +---------------------+
# |         |      | SvcC1D(C1V3orV2D()) |
# | Litware +------> 1 Configuration     +-----+
# |         |      | 1 Dependency        |     |
# +---------+      +---------------------+     |
#                                              |
#                                              |
#                                     +--------v--------+
#                                     | SvcC1V3orV2D()  |
#                                     | 1 Configuration |
#                                     | 0 Dependencies  |
#                                     +-----------------+
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
#        +-> SvcC1D(C1V3orV2D())
#        |     |
#        |     +-> SvcC1D(C1V3orV2D())\Litware
#        |
#        +-> SvcC1V3orV2D()
#
Scenario: V3 dependency configuration
    Given I have enrollment configuration called 'ConfigSvcC1D(C1V3orV2D())'
    And I have enrollment configuration called 'ConfigSvcC1V3orV2D()'
    And the 'ConfigSvcC1D(C1V3orV2D())' enrollment has a dependency on the service tenant called 'SvcC1V3orV2D()' using configuration 'ConfigSvcC1V3orV2D()'
    And the enrollment configuration called 'ConfigSvcC1D(C1V3orV2D())' contains the following Blob Storage configuration items
    | Key                                                                  | Account Name | Container      |
    | V3WithDependencyWithV3V2Migration:BlobContainerConfiguration:myblobs | blobaccount1 | blobcontainer1 |
    And the enrollment configuration called 'ConfigSvcC1V3orV2D()' contains the following Blob Storage configuration items
    | Key                                                  | Account Name | Container      |
    | V3WithV2Migration:BlobContainerConfiguration:myblobs | blobaccount2 | blobcontainer2 |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1D(C1V3orV2D())' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1V3orV2D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1V3orV2D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'V3WithDependencyWithV3V2Migration:BlobContainerConfiguration:myblobs' for the account 'blobaccount1' and container name 'blobcontainer1'
    And a new child tenant called 'SvcC1D(C1V3orV2D())\Litware' of the service tenant called 'SvcC1D(C1V3orV2D())' has been created
    And the tenant called 'SvcC1D(C1V3orV2D())\Litware' should have the id of the tenant called 'SvcC1V3orV2D()' added to its enrollments
    And the tenant called 'SvcC1D(C1V3orV2D())\Litware' should contain blob storage configuration under the key 'V3WithV2Migration:BlobContainerConfiguration:myblobs' for the account 'blobaccount2' and container name 'blobcontainer2'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1V3orV2D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C1V3orV2D())'

Scenario: V2 dependency configuration
    Given I have enrollment configuration called 'ConfigSvcC1D(C1V3orV2D())'
    And I have enrollment configuration called 'ConfigSvcC1V3orV2D()'
    And the 'ConfigSvcC1D(C1V3orV2D())' enrollment has a dependency on the service tenant called 'SvcC1V3orV2D()' using configuration 'ConfigSvcC1V3orV2D()'
    And the enrollment configuration called 'ConfigSvcC1D(C1V3orV2D())' contains the following Blob Storage configuration items
    | Key                                                                  | Account Name | Container      |
    | V3WithDependencyWithV3V2Migration:BlobContainerConfiguration:myblobs | blobaccount1 | blobcontainer1 |
    And the enrollment configuration called 'ConfigSvcC1V3orV2D()' contains the following legacy V2 Blob Storage configuration items
    | Key                                                  | Account Name | Container      |
    | V3WithV2Migration:BlobContainerConfiguration:myblobs | blobaccount2 | blobcontainer2 |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1D(C1V3orV2D())' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1V3orV2D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1V3orV2D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'V3WithDependencyWithV3V2Migration:BlobContainerConfiguration:myblobs' for the account 'blobaccount1' and container name 'blobcontainer1'
    And a new child tenant called 'SvcC1D(C1V3orV2D())\Litware' of the service tenant called 'SvcC1D(C1V3orV2D())' has been created
    And the tenant called 'SvcC1D(C1V3orV2D())\Litware' should have the id of the tenant called 'SvcC1V3orV2D()' added to its enrollments
    And the tenant called 'SvcC1D(C1V3orV2D())\Litware' should contain legacy V2 blob storage configuration under the key 'StorageConfiguration__mycontainer' for the account 'blobaccount2' and container name 'blobcontainer2'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1V3orV2D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C1V3orV2D())'
