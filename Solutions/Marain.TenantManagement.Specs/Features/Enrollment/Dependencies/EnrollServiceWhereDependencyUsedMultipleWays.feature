@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service with a Dependency on a Service That is Also Used in Other Ways
    In order to allow a client to use Marain services with a dependency on a service that is also used in other ways
    As an administrator
    I want to enroll a tenant to use those services

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC0D(C0D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D(C0D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC0D(C1D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC3D(C1D(C1D()))' and used the tenant store to create a service tenant with it
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



# TODO: when this pattern occurs, there is a problem. See the tenant tree below
# Dependency graph:
#
# +---------+      +-----------------+      +------------------+
# |         |      | Service with    |      | Service with     |
# | Litware +------> 1 Configuration +------> 1 Configurations |
# |         |      | 2 Dependencies  |      | 1 Dependency     |
# +----+----+      +-----------------+      +------------------+
#                          |                         |
#                          |                +--------v---------+
#                          |                | Service with     |
#                          +----------------> 1 Configurations |
#                                           | 0 Dependencies   |
#                                           +------------------+
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
#        +-> SvcC1D(SvcC1D(C1D()),C1D())
#        |     |
#        |     +-> SvcC1D(SvcC1D(C1D()),C1D())\Litware
#        |
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\Litware
#        |
#        +-> SvcC1D()
#
# We've got two delegated tenants for Litware, under each of the two service tenants
# shown in the top row of the dependency graph above. Each of these will have its
# own configuration for the service in the bottom row (SvcC1D()). That in these two
# scenarios:
#   1. Litware uses the top left service, which uses the top right service, which
#       uses the bottom right service
#   2. Litware uses the top left service, which uses the bottom right service
# that the bottom right service ends up using different configuration in each case.
# However, our tenant enrollment mechanisms don't make it possible to take advantage
# of this. We can only specify configuration for the bottom right service once when
# enrolling Litware in the top left service, so the two delegated tenants will get
# the same configuration for the bottom right service.
# Is this a problem? It could be. If the bottom right service provides a per-tenant
# namespace, a client could presume that it doesn't need to ensure globally unique
# names. But because two different delegated tenants here will end up with the same
# configuration, they will share a namespace. This could cause trouble.

#no way for the enrollment configuration
# to specify different configuration for use of the bottom right service in the case
# where the top-left service


# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 1 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +----+----+         +------------------+     |
#      |                                       |
#      |                                       |
#      |    +-----------------+       +--------v---------+
#      |    | Service with    |       | Service with     |
#      +----> 1 Configuration +-------> 1 Configurations |
#           | 2 Dependencies  |       | 1 Dependency     |
#           +-----------------+       +------------------+
#                   |                          |
#                   |                 +--------v---------+
#                   |                 | Service with     |
#                   +-----------------> 1 Configurations |
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
#        +-> SvcC0D(C1D())
#        |     |
#        |     +-> SvcC0D(C1D())\Litware
#        |
#        +-> SvcC1D(SvcC1D(C1D()),C1D())
#        |     |
#        |     +-> SvcC1D(SvcC1D(C1D()),C1D())\Litware
#        |
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\Litware
#        |
#        +-> SvcC1D()
# TODO: this has a more complex version of the same problem as the preceding example.

#Scenario: Enrollment of a service two levels of dependency and configuration at all levels where client depends on another service that depends on the middle and lower services
#    Given I have enrollment configuration called 'Config1'
#    And the enrollment configuration called 'Config1' contains the following Blob Storage configuration items
#    | Key                                        | Account Name           | Container                 |
#    | TestServices:C1D():FooBarStore             | fbblobaccount          | fbblobcontainer           |
#    | TestServices:C1D(C1D()):operations         | opsblobaccountindirect | opsblobcontainerindirect  |
#    | TestServices:C3D(C1D(C1D())):manglewurzles | rootvegblobaccount     | mangleworzleblobcontainer |
#	And the enrollment configuration called 'Config1' contains the following Cosmos configuration items
#	| Key                                        | Account Uri          | Database Name | Container Name  |
#	| TestServices:C3D(C1D(C1D())):RootAggregate | rootvegcosmosaccount | rvdb          | aggregatedroots |
#	And the enrollment configuration called 'Config1' contains the following Table Storage configuration items
#	| Key                                    | Account Name   | Table      |
#	| TestServices:C3D(C1D(C1D())):AuditLogs | rvtableaccount | audittable |
#    And I have enrollment configuration called 'Config2'
#    And the enrollment configuration called 'Config2' contains the following Blob Storage configuration items
#    | Key                                | Account Name   | Container        |
#    | TestServices:C1D():FooBarStore     | fbblobaccount2 | fbblobcontainer2 |
#    | TestServices:C1D(C1D()):operations | opsblobaccount | opsblobcontainer |


# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 1 Configuration  +--+
# |         |         | 1 Dependency     |  |
# +----|----+         +------------------+  |
#      |                                    |
#      |                                    |
#      |                           +--------v---------+
#      |                           | Service with     |
#      +---------------------------> 1 Configuration  +--+
#                                  | 1 Dependencies   |  |
#                                  +------------------+  |
#                                                        |
#                                                        |
#                                               +--------v---------+
#                                               | Service with     |
#                                               | 1 Configurations |
#                                               | 0 Dependencies   |
#                                               +------------------+
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
#        +-> SvcC3D(C1D(C1D()))
#        |     |
#        |     +-> SvcC3D(C1D(C1D()))\Litware
#        |
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware
#        |     +-> SvcC1D(C1D())\Litware
#        |
#        +-> SvcC1D()
Scenario: Enrollment of a service two levels of dependency and configuration at all levels where client also uses middle service directly
    Given I have enrollment configuration called 'Top Config'
    And the enrollment configuration called 'Top Config' contains the following Blob Storage configuration items
    | Key                                        | Account Name           | Container                 |
    | TestServices:C1D():FooBarStore             | fbblobaccount          | fbblobcontainer           |
    | TestServices:C1D(C1D()):operations         | opsblobaccountindirect | opsblobcontainerindirect  |
    | TestServices:C3D(C1D(C1D())):manglewurzles | rootvegblobaccount     | mangleworzleblobcontainer |
	And the enrollment configuration called 'Top Config' contains the following Cosmos configuration items
	| Key                                        | Account Uri          | Database Name | Container Name  |
	| TestServices:C3D(C1D(C1D())):RootAggregate | rootvegcosmosaccount | rvdb          | aggregatedroots |
	And the enrollment configuration called 'Top Config' contains the following Table Storage configuration items
	| Key                                    | Account Name   | Table      |
	| TestServices:C3D(C1D(C1D())):AuditLogs | rvtableaccount | audittable |
    And I have enrollment configuration called 'Middle Config'
    And the enrollment configuration called 'Middle Config' contains the following Blob Storage configuration items
    | Key                                | Account Name         | Container              |
    | TestServices:C1D():FooBarStore     | fbblobaccountdirect  | fbblobcontainerdirect  |
    | TestServices:C1D(C1D()):operations | opsblobaccountdirect | opsblobcontainerdirect |
    When I use the tenant store with the enrollment configuration called 'Top Config' to enroll the tenant called 'Litware' in the service called 'SvcC3D(C1D(C1D()))'
    And I use the tenant store with the enrollment configuration called 'Middle Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1D())'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC3D(C1D(C1D()))' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C3D(C1D(C1D())):manglewurzles' for the account 'rootvegblobaccount' and container name 'mangleworzleblobcontainer'
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D()):operations' for the account 'opsblobaccountdirect' and container name 'opsblobcontainerdirect'
    And the tenant called 'Litware' should contain Cosmos configuration under the key 'TestServices:C3D(C1D(C1D())):RootAggregate' with database name 'rvdb' and container name 'aggregatedroots'
    And the tenant called 'Litware' should contain table storage configuration under the key 'TestServices:C3D(C1D(C1D())):AuditLogs' for the account 'rvtableaccount' and table name 'audittable'
    And a new child tenant called 'SvcC3D(C1D(C1D()))\Litware' of the service tenant called 'SvcC3D(C1D(C1D()))' has been created
    And a new child tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC1D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC1D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccountdirect' and container name 'fbblobcontainerdirect'
    And a new child tenant called 'SvcC1D(C1D())\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC3D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC3D(C1D(C1D()))\Litware' set as the delegated tenant for the service called 'SvcC3D(C1D(C1D()))'
    And the tenant called 'SvcC3D(C1D(C1D()))\Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D()):operations' for the account 'opsblobaccountindirect' and container name 'opsblobcontainerindirect'
    And the tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC3D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D())'
    And the tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
