@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll service that has dependency which has a further dependency
    In order to allow a client to use a Marain service that depends on another Marain service that depends on yet another Marain service
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC0D(C0D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC0D(C0D(C0D()))' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC3D(C1D(C1D()))' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+         +--------------------+
# |         |         | SvcC0D(C0D(C0D())) |
# | Litware +---------> 0 Configurations   +--+
# |         |         | 1 Dependency       |  |
# +---------+         +--------------------+  |
#                                             |
#                                             |
#                                    +--------v---------+
#                                    | SvcC0D(C0D())    |
#                                    | 0 Configurations +--+
#                                    | 1 Dependencies   |  |
#                                    +------------------+  |
#                                                          |
#                                                          |
#                                                 +--------v---------+
#                                                 | SvcC0D()         |
#                                                 | 0 Configurations |
#                                                 | 0 Dependencies   |
#                                                 +------------------+
#
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
#        +-> SvcC0D(C0D(C0D()))
#        |     |
#        |     +-> SvcC0D(C0D(C0D()))\Litware
#        |
#        +-> SvcC0D(C0D())
#        |     |
#        |     +-> SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware
#        |
#        +-> SvcC0D()
#
Scenario: Enrollment of a service two levels of dependency and no configuration at any level supplying empty configuration
    Given I have enrollment configuration called 'ConfigSvcC0D(C0D(C0D()))'
    And I have enrollment configuration called 'ConfigSvcC0D(C0D())'
    And I have enrollment configuration called 'ConfigSvcC0D()'
    And the 'ConfigSvcC0D(C0D(C0D()))' enrollment has a dependency on the service tenant called 'SvcC0D(C0D(C0D()))' using configuration 'ConfigSvcC0D(C0D())'
    And the 'ConfigSvcC0D(C0D())' enrollment has a dependency on the service tenant called 'SvcC0D(C0D())' using configuration 'ConfigSvcC0D()'
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC0D(C0D(C0D()))' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D(C0D()))'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D(C0D()))' added to its enrollments
    And a new child tenant called 'SvcC0D(C0D(C0D()))\Litware' of the service tenant called 'SvcC0D(C0D(C0D()))' has been created
    And a new child tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D(C0D()))\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D(C0D()))'
    And the tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D())'

# Same as above again but with no config supplied
Scenario: Enrollment of a service two levels of dependency and no configuration at any level supplying no dependency configuration
    Given I have enrollment configuration called 'ConfigSvcC0D(C0D(C0D()))'
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC0D(C0D(C0D()))' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D(C0D()))'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D(C0D()))' added to its enrollments
    And a new child tenant called 'SvcC0D(C0D(C0D()))\Litware' of the service tenant called 'SvcC0D(C0D(C0D()))' has been created
    And a new child tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D(C0D()))\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D(C0D()))'
    And the tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D())'


# Dependency graph:
#
# +---------+         +--------------------+
# |         |         | SvcC3D(C1D(C1D())) |
# | Litware +---------> 3 Configurations   +--+
# |         |         | 1 Dependency       |  |
# +---------+         +--------------------+  |
#                                             |
#                                             |
#                                    +--------v---------+
#                                    | Service with     |
#                                    | 1 Configuration  +--+
#                                    | 1 Dependencies   |  |
#                                    +------------------+  |
#                                                          |
#                                                          |
#                                                 +--------v---------+
#                                                 | Service with     |
#                                                 | 1 Configurations |
#                                                 | 0 Dependencies   |
#                                                 +------------------+
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
#        |
#        +-> SvcC1D()
#
Scenario: Enrollment of a service two levels of dependency and configuration at all levels
    Given I have enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))'
    And I have enrollment configuration called 'ConfigSvcC1D(C1D())'
    And I have enrollment configuration called 'ConfigSvcC1D()'
    And the 'ConfigSvcC3D(C1D(C1D()))' enrollment has a dependency on the service tenant called 'SvcC1D(C1D())' using configuration 'ConfigSvcC1D(C1D())'
    And the 'ConfigSvcC1D(C1D())' enrollment has a dependency on the service tenant called 'SvcC1D()' using configuration 'ConfigSvcC1D()'
    And the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' contains the following Blob Storage configuration items
    | Key                                        | Account Name       | Container                 |
    | TestServices:C3D(C1D(C1D())):manglewurzles | rootvegblobaccount | mangleworzleblobcontainer |
	And the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' contains the following Cosmos configuration items
	| Key                                        | Account Uri          | Database Name | Container Name  |
	| TestServices:C3D(C1D(C1D())):RootAggregate | rootvegcosmosaccount | rvdb          | aggregatedroots |
	And the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' contains the following Table Storage configuration items
	| Key                                    | Account Name   | Table      |
	| TestServices:C3D(C1D(C1D())):AuditLogs | rvtableaccount | audittable |
    And the enrollment configuration called 'ConfigSvcC1D(C1D())' contains the following Blob Storage configuration items
    | Key                                        | Account Name       | Container                 |
    | TestServices:C1D(C1D()):operations         | opsblobaccount     | opsblobcontainer          |
    And the enrollment configuration called 'ConfigSvcC1D()' contains the following Blob Storage configuration items
    | Key                                        | Account Name       | Container                 |
    | TestServices:C1D():FooBarStore             | fbblobaccount      | fbblobcontainer           |
    When I use the tenant store with the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' to enroll the tenant called 'Litware' in the service called 'SvcC3D(C1D(C1D()))'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC3D(C1D(C1D()))' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C3D(C1D(C1D())):manglewurzles' for the account 'rootvegblobaccount' and container name 'mangleworzleblobcontainer'
    And the tenant called 'Litware' should contain Cosmos configuration under the key 'TestServices:C3D(C1D(C1D())):RootAggregate' with database name 'rvdb' and container name 'aggregatedroots'
    And the tenant called 'Litware' should contain table storage configuration under the key 'TestServices:C3D(C1D(C1D())):AuditLogs' for the account 'rvtableaccount' and table name 'audittable'
    And a new child tenant called 'SvcC3D(C1D(C1D()))\Litware' of the service tenant called 'SvcC3D(C1D(C1D()))' has been created
    And a new child tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC3D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC3D(C1D(C1D()))\Litware' set as the delegated tenant for the service called 'SvcC3D(C1D(C1D()))'
    And the tenant called 'SvcC3D(C1D(C1D()))\Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D()):operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
    And the tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC3D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D())'
    And the tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
