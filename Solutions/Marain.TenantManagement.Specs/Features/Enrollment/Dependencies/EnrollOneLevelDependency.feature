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
# |         |         | SvcC0D(C0D())    |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +---------+         +------------------+     |
#                                              |
#                                              |
#                                     +--------v---------+
#                                     | SvcC0D()         |
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
    Given I have enrollment configuration called 'ConfigSvcC0D(C0D)'
    And I have enrollment configuration called 'ConfigSvcC0D()'
    And the 'ConfigSvcC0D(C0D)' enrollment has a dependency on the service tenant called 'SvcC0D()' using configuration 'ConfigSvcC0D()'
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC0D(C0D)' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And a new child tenant called 'SvcC0D(C0D())\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D())'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | SvcC0D(C1D())    |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +---------+         +------------------+     |
#                                              |
#                                              |
#                                     +--------v--------+
#                                     | SvcC1D()        |
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
    Given I have enrollment configuration called 'ConfigC0D(C1D())'
    And I have enrollment configuration called 'ConfigSvcC1D()'
    And the 'ConfigC0D(C1D())' enrollment has a dependency on the service tenant called 'SvcC1D()' using configuration 'ConfigSvcC1D()'
    And the enrollment configuration called 'ConfigSvcC1D()' contains the following Blob Storage configuration items
    | Key                            | Account Name  | Container       |
    | TestServices:C1D():FooBarStore | fbblobaccount | fbblobcontainer |
    When I use the tenant store with the enrollment configuration called 'ConfigC0D(C1D())' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C1D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C1D())' added to its enrollments
    And a new child tenant called 'SvcC0D(C1D())\Litware' of the service tenant called 'SvcC0D(C1D())' has been created
    And the tenant called 'SvcC0D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
    And the tenant called 'SvcC0D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C1D())\Litware' set as the delegated tenant for the service called 'SvcC0D(C1D())'


# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | SvcC1D(C0D())   |
# | Litware +---------> 1 Configuration +------+
# |         |         | 1 Dependency    |      |
# +---------+         +-----------------+      |
#                                              |
#                                              |
#                                     +--------v---------+
#                                     | SvcC0D()         |
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
# We have two versions of this scenario:
#   1) the enrollment configuration includes a section for the dependency, with no config in (because none is required)
#   2) the enrollment configuration does not include a section for the dependency (because no config is requied for it)
# We test both because they require slightly different validation handling.
Scenario: Enrollment of a service with its own configuration and a dependency not requiring configuration empty dependency config supplied
    Given I have enrollment configuration called 'ConfigSvcC1D(C0D())'
    And I have enrollment configuration called 'ConfigSvcC0D()'
    And the 'ConfigSvcC1D(C0D())' enrollment has a dependency on the service tenant called 'SvcC0D()' using configuration 'ConfigSvcC0D()'
    And the enrollment configuration called 'ConfigSvcC1D(C0D())' contains the following Blob Storage configuration items
    | Key                                | Account Name      | Container        |
    | TestServices:C1D(C0D()):operations | opsblobaccount    | opsblobcontainer |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1D(C0D())' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C0D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C0D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And a new child tenant called 'SvcC1D(C0D())\Litware' of the service tenant called 'SvcC1D(C0D())' has been created
    And the tenant called 'SvcC1D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C0D())'

Scenario: Enrollment of a service with its own configuration and a dependency not requiring configuration no dependency config supplied
    Given I have enrollment configuration called 'ConfigSvcC1D(C0D())'
    And the enrollment configuration called 'ConfigSvcC1D(C0D())' contains the following Blob Storage configuration items
    | Key                                | Account Name      | Container        |
    | TestServices:C1D(C0D()):operations | opsblobaccount    | opsblobcontainer |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1D(C0D())' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C0D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C0D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And a new child tenant called 'SvcC1D(C0D())\Litware' of the service tenant called 'SvcC1D(C0D())' has been created
    And the tenant called 'SvcC1D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C0D())'

# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | SvcC1D(C1D())   |
# | Litware +---------> 1 Configuration +-------+
# |         |         | 1 Dependency    |       |
# +---------+         +-----------------+       |
#                                               |
#                                               |
#                                      +--------v--------+
#                                      | SvcC1D()        |
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
    Given I have enrollment configuration called 'ConfigSvcC1D(C1D())'
    And I have enrollment configuration called 'ConfigSvcC1D()'
    And the 'ConfigSvcC1D(C1D())' enrollment has a dependency on the service tenant called 'SvcC1D()' using configuration 'ConfigSvcC1D()'
    And the enrollment configuration called 'ConfigSvcC1D(C1D())' contains the following Blob Storage configuration items
    | Key                                | Account Name   | Container        |
    | TestServices:C1D(C1D()):operations | opsblobaccount | opsblobcontainer |
    And the enrollment configuration called 'ConfigSvcC1D()' contains the following Blob Storage configuration items
    | Key                                | Account Name   | Container        |
    | TestServices:C1D():FooBarStore     | fbblobaccount  | fbblobcontainer  |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC1D(C1D())' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And a new child tenant called 'SvcC1D(C1D())\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC1D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC1D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D())'
