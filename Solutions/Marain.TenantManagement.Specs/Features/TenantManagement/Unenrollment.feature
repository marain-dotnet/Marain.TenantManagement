@perScenarioContainer
@useInMemoryTenantProvider

Feature: Unenrollment
    In order to disallow a currently allowed a client from using a Marain service
    As an administrator
    I want to unenroll a tenant from a service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D()'
    And I have loaded the manifest called 'ServiceManifestC1D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D()'
    And I have loaded the manifest called 'ServiceManifestC1D(C1D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D(C1D())'
    And I have loaded the manifest called 'WorkflowServiceManifest'
    And I have used the tenant store to create a service tenant with manifest 'WorkflowServiceManifest'
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
    | Key         | Account Name | Container     |
    | fooBarStore | blobaccount  | blobcontainer |
    And I have used the tenant store with the enrollment configuration called 'FooBar config' to enroll the tenant called 'Litware' in the service called 'SvcC1D()'
    When I use the tenant store to unenroll the tenant called 'Litware' from the service called 'SvcC1D()'
    Then the tenant called 'Litware' should not have the id of the tenant called 'SvcC1D()' in its enrollments
    And the tenant called 'Litware' should not contain blob storage configuration under key 'foobar'

Scenario: Unenrollment with multiple levels of dependency and with the client tenant remaining directly enrolled in one of the dependent services
    # Dependency graph prior to unenrollment:
    #
    # +---------+        +------------+
    # |         |        |            |
    # | Litware +--------> WORKFLOW   +------+
    # |         |        |            |      |
    # +-----+---+        +------------+      |
    #       |                                |
    #       |                                |
    #       |                          +-----v------+
    #       |                          |            |
    #       +--------------------------> OPERATIONS +------+
    #                                  |            |      |
    #                                  +------------+      |
    #                                                      |
    #                                                      |
    #                                                +-----v------+
    #                                                |            |
    #                                                | FOOBAR     |
    #                                                |            |
    #                                                +------------+
    #
    # Tenant tree prior to unenrollment:
    #
    # Root tenant
    #  |
    #  +-> Client Tenants
    #  |     |
    #  |     +-> Litware
    #  |
    #  +-> Service Tenants
    #        |
    #        +-> Workflow v1
    #        |     |
    #        |     +-> Workflow v1\Litware
    #        |
    #        +-> Operations v1
    #        |     |
    #        |     +-> Operations v1\Workflow v1\Litware
    #        |     |
    #        |     +-> Operations v1\Litware
    #        |
    #        +-> FooBar
    #
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
    #        +-> Workflow v1
    #        |
    #        +-> Operations v1
    #        |     |
    #        |     +-> Operations v1\Litware
    #        |
    #        +-> FooBar
    #
    Given I have enrollment configuration called 'Workflow config'
    And the enrollment configuration called 'Workflow config' contains the following Blob Storage configuration items
    | Key                                                    | Account Name   | Container        |
    | fooBarStore                                            | fbblobaccount  | fbblobcontainer  |
    | MarainOperations:BlobContainerConfiguration:operations | opsblobaccount | opsblobcontainer |
    And the enrollment configuration called 'Workflow config' contains the following Cosmos configuration items
    | Key                                                     | Account Uri | Database Name | Container Name      |
    | MarainWorkflow:CosmosContainerConfiguration:Definitions | wfaccount   | wfdb          | wfcontainer         |
    | MarainWorkflow:CosmosContainerConfiguration:Instances   | wfaccount   | wfdb          | wfinstancecontainer |
    And the enrollment configuration called 'Workflow config' contains the following Table Storage configuration items
    | Key                                                 | Account Name   | Table   |
    | MarainWorkflow:BlobContainerConfiguration:AuditLogs | fbtableaccount | fbtable |
    And I have enrollment configuration called 'Operations config'
    And the enrollment configuration called 'Operations config' contains the following Blob Storage configuration items
    | Key                                                    | Account Name    | Container         |
    | fooBarStore                                            | fbblobaccount2  | fbblobcontainer2  |
    | MarainOperations:BlobContainerConfiguration:operations | opsblobaccount2 | opsblobcontainer2 |
    And I have used the tenant store with the enrollment configuration called 'Workflow config' to enroll the tenant called 'Litware' in the service called 'Workflow v1'
    And I have used the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
    When I use the tenant store to unenroll the tenant called 'Litware' from the service called 'Workflow v1'
    Then the tenant called 'Litware' should not have the id of the tenant called 'Workflow v1' in its enrollments
    And the tenant called 'Litware' should not contain Cosmos configuration for a Cosmos container definition under the key 'MarainWorkflow:CosmosContainerConfiguration:Definitions'
    And the tenant called 'Litware' should not contain Cosmos configuration for a Cosmos container definition under the key 'MarainWorkflow:CosmosContainerConfiguration:Instances'
    And the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
    And the tenant called 'Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for the account 'opsblobaccount2' and container name 'opsblobcontainer2'
    And there should not be a child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1'
    And the tenant called 'Litware' should not have a delegated tenant for the service called 'Workflow v1'
    And there should not be a child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1'
    And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
    And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
    And the tenant called 'Operations v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for the account 'fbblobaccount2' and container name 'fbblobcontainer2'
    And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'