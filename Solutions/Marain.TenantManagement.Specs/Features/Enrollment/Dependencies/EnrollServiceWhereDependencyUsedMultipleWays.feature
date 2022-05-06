@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service with a Dependency on a Service That is Also Used in Other Ways
    In order to allow a client to use Marain services with a dependency on a service that is also used in other ways
    As an administrator
    I want to enroll a tenant to use those services

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D()'
    And I have loaded the manifest called 'ServiceManifestC1D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D()'
    And I have loaded the manifest called 'ServiceManifestC0D(C0D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D(C0D())'
    And I have loaded the manifest called 'ServiceManifestC0D(C1D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D(C1D())'
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D(C1D())'
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +----+----+         +------------------+     |
#      |                                       |
#      |                                       |
#      |                              +--------v---------+
#      |                              | Service with     |
#      +------------------------------> 0 Configurations |
#                                     | 0 Dependencies   |
#                                     +------------------+
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
Scenario: Service used directly and as dependency with no configuration
    When I use the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D())'
    And I use the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC0D()'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And a new child tenant called 'SvcC0D(C0D())\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D())'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +----+----+         +------------------+     |
#      |                                       |
#      |                                       |
#      |    +-----------------+       +--------v---------+
#      |    | Service with    |       | Service with     |
#      +----> 1 Configuration +-------> 0 Configurations |
#           | 1 Dependency    |       | 0 Dependencies   |
#           +-----------------+       +------------------+
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
#        +-> SvcC1D(C0D())
#        |     |
#        |     +-> SvcC1D(C0D())\Litware
#        |
#        +-> SvcC0D()
Scenario: Service with no configuration used as dependency of two different services
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                                | Account Name   | Container        |
    | TestServices:C1D(C0D()):operations | opsblobaccount | opsblobcontainer |
    When I use the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D())'
    And I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C0D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C0D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C0D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And a new child tenant called 'SvcC0D(C0D())\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And a new child tenant called 'SvcC1D(C0D())\Litware' of the service tenant called 'SvcC1D(C0D())' has been created
    And the tenant called 'SvcC1D(C0D())\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +----+----+         +------------------+     |
#      |                                       |
#      |                                       |
#      |    +-----------------+       +--------v---------+
#      |    | Service with    |       | Service with     |
#      +----> 1 Configuration +-------> 1 Configurations |
#           | 1 Dependency    |       | 0 Dependencies   |
#           +-----------------+       +------------------+
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
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\Litware
#        |
#        +-> SvcC1D()
# TODO. (Requires multiple configs.)
Scenario: Service with configuration used as dependency of two different services
    Given I have enrollment configuration called 'Config1'
    And the enrollment configuration called 'Config1' contains the following Blob Storage configuration items
    | Key                            | Account Name   | Container        |
    | TestServices:C1D():FooBarStore | opsblobaccount1 | opsblobcontainer1 |
    And I have enrollment configuration called 'Config2'
    And the enrollment configuration called 'Config2' contains the following Blob Storage configuration items
    | Key                                | Account Name   | Container        |
    | TestServices:C1D():FooBarStore     | fbblobaccount2 | fbblobcontainer2 |
    | TestServices:C1D(C1D()):operations | opsblobaccount | opsblobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config1' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C1D())'
    And I use the tenant store with the enrollment configuration called 'Config2' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1D())'



# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +----+----+         +------------------+     |
#      |                                       |
#      |                                       |
#      |    +-----------------+       +--------v---------+
#      |    | Service with    |       | Service with     |
#      +----> 1 Configuration +-------> 1 Configurations |
#           | 1 Dependency    |       | 1 Dependencies   |
#           +-----------------+       +------------------+
#                   |                          |
#                   |                 +--------v---------+
#                   |                 | Service with     |
#                   +-----------------> 1 Configurations |
#                                     | 0 Dependencies   |
#                                     +------------------+

# TODO. See example in Enrollment.feature