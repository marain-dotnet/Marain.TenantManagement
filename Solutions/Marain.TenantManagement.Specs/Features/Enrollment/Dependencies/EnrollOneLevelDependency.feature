@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service That Has One Dependency with No Further Dependencies
    In order to allow a client to use a Marain service that depends on another Marain service
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC0D(C0D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC0D(C1D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D(C0D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +---------+         +------------------+     |
#                                              |
#                                              |
#                                     +--------v---------+
#                                     | Service with     |
#                                     | 0 Configurations |
#                                     | 0 Dependencies   |
#                                     +------------------+
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
#        +-> SvcC0D(C0D())
#        |     |
#        |     +-> SvcC0D(C0D())\Litware
#        |
#        +-> SvcC0D()
#
Scenario: Enrollment of a service with no configuration and a dependency requiring no configuration
    Given I have enrollment configuration called 'Config'
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And a new child tenant called 'SvcC0D(C0D())\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D())'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +---------+         +------------------+     |
#                                              |
#                                              |
#                                     +--------v--------+
#                                     | Service with    |
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
#        +-> SvcC0D(C1D())
#        |     |
#        |     +-> SvcC0D(C1D())\Litware
#        |
#        +-> SvcC1D()
#
Scenario: Enrollment of a service with no configuration and a dependency requiring configuration
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                            | Account Name  | Container       |
    | TestServices:C1D():FooBarStore | fbblobaccount | fbblobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C1D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C1D())' added to its enrollments
    And a new child tenant called 'SvcC0D(C1D())\Litware' of the service tenant called 'SvcC0D(C1D())' has been created
    And the tenant called 'SvcC0D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
    And the tenant called 'SvcC0D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C1D())\Litware' set as the delegated tenant for the service called 'SvcC0D(C1D())'


# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | Service with    |
# | Litware +---------> 1 Configuration +------+
# |         |         | 1 Dependency    |      |
# +---------+         +-----------------+      |
#                                              |
#                                              |
#                                     +--------v---------+
#                                     | Service with     |
#                                     | 0 Configurations |
#                                     | 0 Dependencies   |
#                                     +------------------+
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
#        +-> SvcC1D(C0D())
#        |     |
#        |     +-> Service with 1 Configuration, 1 Dependency: (0 Configuration, 0 Dependencies)\Litware
#        |
#        +-> Service with 0 Configuration, 0 Dependencies
Scenario: Enrollment of a service with its own configuration and a dependency not requiring configuration
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                                | Account Name      | Container        |
    | TestServices:C1D(C0D()):operations | opsblobaccount    | opsblobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C0D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C0D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And a new child tenant called 'SvcC1D(C0D())\Litware' of the service tenant called 'SvcC1D(C0D())' has been created
    And the tenant called 'SvcC1D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C0D())'

# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | Service with    |
# | Litware +---------> 1 Configuration +-------+
# |         |         | 1 Dependency    |       |
# +---------+         +-----------------+       |
#                                               |
#                                               |
#                                      +--------v--------+
#                                      | Service with    |
#                                      | 1 Configuration |
#                                      | 0 Dependencies  |
#                                      +-----------------+
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
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\Litware
#        |
#        +-> SvcC1D()
Scenario: Enrollment of a service with its own configuration and a dependency requiring configuration
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                                | Account Name   | Container        |
    | TestServices:C1D():FooBarStore     | fbblobaccount  | fbblobcontainer  |
    | TestServices:C1D(C1D()):operations | opsblobaccount | opsblobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And a new child tenant called 'SvcC1D(C1D())\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC1D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC1D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D())'
