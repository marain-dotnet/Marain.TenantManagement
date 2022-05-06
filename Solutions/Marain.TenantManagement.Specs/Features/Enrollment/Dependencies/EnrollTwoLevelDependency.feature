@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll service that has dependency which has a further dependency
    In order to allow a client to use a Marain service that depends on another Marain service that depends on yet another Marain service
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D()'
    And I have loaded the manifest called 'ServiceManifestC1D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D()'
    And I have loaded the manifest called 'ServiceManifestC0D(C0D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D(C0D())'
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D(C1D())'
    And I have loaded the manifest called 'ServiceManifestC0D(C0D(C0D()))'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D(C0D(C0D()))'
    And I have loaded the manifest called 'ServiceManifestC1D(C1D(C1D()))'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D(C1D(C1D()))'
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +--+
# |         |         | 1 Dependency     |  |
# +---------+         +------------------+  |
#                                           |
#                                           |
#                                  +--------v---------+
#                                  | Service with     |
#                                  | 0 Configurations +--+
#                                  | 1 Dependencies   |  |
#                                  +------------------+  |
#                                                        |
#                                                        |
#                                               +--------v---------+
#                                               | Service with     |
#                                               | 0 Configurations |
#                                               | 0 Dependencies   |
#                                               +------------------+
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
Scenario: Enrollment of a service two levels of dependency and no configuration at any level
    Given I have enrollment configuration called 'Config'
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC0D(C0D(C0D()))'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D(C0D()))' added to its enrollments
    And a new child tenant called 'SvcC0D(C0D(C0D()))\Litware' of the service tenant called 'SvcC0D(C0D(C0D()))' has been created
    And a new child tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' of the service tenant called 'SvcC0D(C0D())' has been created
    And the tenant called 'SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D(C0D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC0D(C0D(C0D()))\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D(C0D()))'
    And the tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments
    And the tenant called 'SvcC0D(C0D(C0D()))\Litware' should have the id of the tenant called 'SvcC0D(C0D())\SvcC0D(C0D(C0D()))\Litware' set as the delegated tenant for the service called 'SvcC0D(C0D())'


# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +--+
# |         |         | 1 Dependency     |  |
# +---------+         +------------------+  |
#                                           |
#                                           |
#                                  +--------v---------+
#                                  | Service with     |
#                                  | 0 Configurations +--+
#                                  | 1 Dependencies   |  |
#                                  +------------------+  |
#                                                        |
#                                                        |
#                                               +--------v---------+
#                                               | Service with     |
#                                               | 0 Configurations |
#                                               | 0 Dependencies   |
#                                               +------------------+
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
#        +-> SvcC1D(C1D(C1D()))
#        |     |
#        |     +-> SvcC1D(C1D(C1D()))\Litware
#        |
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\SvcC1D(C1D(C1D()))\Litware
#        |
#        +-> SvcC1D()
#
Scenario: Enrollment of a service two levels of dependency and configuration at all levels
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                                        | Account Name       | Container                 |
    | TestServices:C1D():FooBarStore             | fbblobaccount      | fbblobcontainer           |
    | TestServices:C1D(C1D()):operations         | opsblobaccount     | opsblobcontainer          |
    | TestServices:C1D(C1D(C1D())):manglewurzles | rootvegblobaccount | mangleworzleblobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1D(C1D()))'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D(C1D()))' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D(C1D())):manglewurzles' for the account 'rootvegblobaccount' and container name 'mangleworzleblobcontainer'
    And a new child tenant called 'SvcC1D(C1D(C1D()))\Litware' of the service tenant called 'SvcC1D(C1D(C1D()))' has been created
    And a new child tenant called 'SvcC1D(C1D())\SvcC1D(C1D(C1D()))\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC1D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D(C1D()))\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D(C1D()))'
    And the tenant called 'SvcC1D(C1D())\SvcC1D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC1D(C1D(C1D()))\Litware' should have the id of the tenant called 'SvcC1D(C1D())\SvcC1D(C1D(C1D()))\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D())'
