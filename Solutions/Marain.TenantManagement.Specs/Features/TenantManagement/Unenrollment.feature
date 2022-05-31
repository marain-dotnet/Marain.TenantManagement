@perScenarioContainer
@useInMemoryTenantProvider

Feature: Unenrollment
    In order to disallow a currently allowed a client from using a Marain service
    As an administrator
    I want to unenroll a tenant from a service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D()' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'ServiceManifestC3D(C1D(C1D()))' and used the tenant store to create a service tenant with it
    And I have loaded the manifest called 'WorkflowServiceManifest' and used the tenant store to create a service tenant with it
    And I have used the tenant store to create a new client tenant called 'Litware'
    And I have used the tenant store to create a new client tenant called 'Contoso'

Scenario: Unenroll from a service that the client has not previously enrolled in
    When I use the tenant store to unenroll the tenant called 'Litware' from the service called 'SvcC0D()'
    Then an 'InvalidOperationException' is thrown

Scenario: Basic unenrollment with no dependencies or configuration
    Given I have used the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC0D()'
    When I use the tenant store to unenroll the tenant called 'Litware' from the service called 'SvcC0D()'
    Then the tenant called 'Litware' should not have the id of the tenant called 'SvcC0D()' in its enrollments

Scenario: Basic unenrollment with configuration
    Given I have enrollment configuration called 'FooBar config'
    And the enrollment configuration called 'FooBar config' contains the following Blob Storage configuration items
    | Key                            | Account Name | Container     |
    | TestServices:C1D():FooBarStore | blobaccount  | blobcontainer |
    And I have used the tenant store with the enrollment configuration called 'FooBar config' to enroll the tenant called 'Litware' in the service called 'SvcC1D()'
    When I use the tenant store to unenroll the tenant called 'Litware' from the service called 'SvcC1D()'
    Then the tenant called 'Litware' should not have the id of the tenant called 'SvcC1D()' in its enrollments
    And the tenant called 'Litware' should not contain blob storage configuration under key 'TestServices:C1D():FooBarStore'

# Dependency graph prior to unenrollment:
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
# Tenant tree prior to unenrollment:
#
## Root tenant
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

# Expected tenant tree after unenrollment:
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
#        |
#        +-> SvcC1D(C1D())
#        |     |
#        |     +-> SvcC1D(C1D())\Litware
#        |
#        +-> SvcC1D()
#
Scenario: Unenrollment with multiple levels of dependency and with the client tenant remaining directly enrolled in one of the dependent services
    Given I have enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))'
    And I have enrollment configuration called 'ConfigSvcC1D(C1D())Direct'
    And I have enrollment configuration called 'ConfigSvcC1D(C1D())Indirect'
    And I have enrollment configuration called 'ConfigSvcC1D()Indirect'
    And I have enrollment configuration called 'ConfigSvcC1D()DoublyIndirect'
    And the 'ConfigSvcC3D(C1D(C1D()))' enrollment has a dependency on the service tenant called 'SvcC1D(C1D())' using configuration 'ConfigSvcC1D(C1D())Indirect'
    And the 'ConfigSvcC1D(C1D())Indirect' enrollment has a dependency on the service tenant called 'SvcC1D()' using configuration 'ConfigSvcC1D()DoublyIndirect'
    And the 'ConfigSvcC1D(C1D())Direct' enrollment has a dependency on the service tenant called 'SvcC1D()' using configuration 'ConfigSvcC1D()Indirect'
    And the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' contains the following Blob Storage configuration items
    | Key                                        | Account Name           | Container                 |
    | TestServices:C3D(C1D(C1D())):manglewurzles | rootvegblobaccount     | mangleworzleblobcontainer |
	And the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' contains the following Cosmos configuration items
	| Key                                        | Account Uri          | Database Name | Container Name  |
	| TestServices:C3D(C1D(C1D())):RootAggregate | rootvegcosmosaccount | rvdb          | aggregatedroots |
	And the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' contains the following Table Storage configuration items
	| Key                                    | Account Name   | Table      |
	| TestServices:C3D(C1D(C1D())):AuditLogs | rvtableaccount | audittable |
    And the enrollment configuration called 'ConfigSvcC1D(C1D())Direct' contains the following Blob Storage configuration items
    | Key                                | Account Name         | Container              |
    | TestServices:C1D(C1D()):operations | opsblobaccountdirect | opsblobcontainerdirect |
    And the enrollment configuration called 'ConfigSvcC1D(C1D())Indirect' contains the following Blob Storage configuration items
    | Key                                        | Account Name           | Container                 |
    | TestServices:C1D(C1D()):operations         | opsblobaccountindirect | opsblobcontainerindirect  |
    And the enrollment configuration called 'ConfigSvcC1D()Indirect' contains the following Blob Storage configuration items
    | Key                            | Account Name        | Container             |
    | TestServices:C1D():FooBarStore | fbblobaccountdirect | fbblobcontainerdirect |
    And the enrollment configuration called 'ConfigSvcC1D()DoublyIndirect' contains the following Blob Storage configuration items
    | Key                            | Account Name          | Container               |
    | TestServices:C1D():FooBarStore | fbblobaccountindirect | fbblobcontainerindirect |
    And I have used the tenant store with the enrollment configuration called 'ConfigSvcC3D(C1D(C1D()))' to enroll the tenant called 'Litware' in the service called 'SvcC3D(C1D(C1D()))'
    And I have used the tenant store with the enrollment configuration called 'ConfigSvcC1D(C1D())Direct' to enroll the tenant called 'Litware' in the service called 'SvcC1D(C1D())'
    When I use the tenant store to unenroll the tenant called 'Litware' from the service called 'SvcC3D(C1D(C1D()))'
    Then the tenant called 'Litware' should not have the id of the tenant called 'SvcC3D(C1D(C1D()))' in its enrollments
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())' added to its enrollments
    And the tenant called 'Litware' should not contain Cosmos configuration for a Cosmos container definition under the key 'TestServices:C3D(C1D(C1D())):RootAggregate'
    And the tenant called 'Litware' should contain blob storage configuration under the key 'TestServices:C1D(C1D()):operations' for the account 'opsblobaccountdirect' and container name 'opsblobcontainerdirect'
    And there should not be a child tenant called 'SvcC3D(C1D(C1D()))\Litware' of the service tenant called 'SvcC3D(C1D(C1D()))'
    And the tenant called 'Litware' should not have a delegated tenant for the service called 'SvcC3D(C1D(C1D()))'
    And there should not be a child tenant called 'SvcC1D(C1D())\SvcC3D(C1D(C1D()))\Litware' of the service tenant called 'SvcC1D(C1D())'
    And a new child tenant called 'SvcC1D(C1D())\Litware' of the service tenant called 'SvcC1D(C1D())' has been created
    And the tenant called 'SvcC1D(C1D())\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'SvcC1D(C1D())\Litware' should contain blob storage configuration under the key 'TestServices:C1D():FooBarStore' for the account 'fbblobaccountdirect' and container name 'fbblobcontainerdirect'
    And the tenant called 'Litware' should have the id of the tenant called 'SvcC1D(C1D())\Litware' set as the delegated tenant for the service called 'SvcC1D(C1D())'
    And the tenant called 'Litware' should not contain blob storage configuration under key 'TestServices:C3D(C1D(C1D())):manglewurzles'
    And the tenant called 'Litware' should not contain Cosmos configuration for a Cosmos container definition under the key 'TestServices:C3D(C1D(C1D())):RootAggregate'
    And the tenant called 'Litware' should not contain table storage configuration under key 'TestServices:C3D(C1D(C1D())):AuditLogs'
    And the tenant called 'Litware' should not have a delegated tenant for the service called 'SvcC3D(C1D(C1D()))'
