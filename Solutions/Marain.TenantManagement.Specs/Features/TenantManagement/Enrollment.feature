@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enrollment
	In order to allow a client to use a Marain service
	As an administrator
	I want to enroll a tenant to use that service

# TODO:
# split this into multiple files separating out the various cases
# add legacy handling
#	- V2 required config with V2 config supplied
#	- V3 required config with V3 config supplied
#	- V3 or V2 required config with V3 config supplied
#	- V3 or V2 required config with V2 config supplied (do we add the V3 version as well)
# and disallowed cases:
#	- V2 required config with V3 supplied
#	- V3 required config with V2 supplied (or can we auto-upgrade?)

Background:
	Given the tenancy provider has been initialised for use with Marain
	And I have loaded the manifest called 'SimpleManifestWithNoDependenciesOrConfiguration'
	And I have used the tenant store to create a service tenant with manifest 'SimpleManifestWithNoDependenciesOrConfiguration'
	And I have loaded the manifest called 'FooBarServiceManifest'
	And I have used the tenant store to create a service tenant with manifest 'FooBarServiceManifest'
	And I have loaded the manifest called 'OperationsServiceManifest'
	And I have used the tenant store to create a service tenant with manifest 'OperationsServiceManifest'
	And I have loaded the manifest called 'WorkflowServiceManifest'
	And I have used the tenant store to create a service tenant with manifest 'WorkflowServiceManifest'
	And I have used the tenant store to create a new client tenant called 'Litware'
	And I have used the tenant store to create a new client tenant called 'Contoso'

Scenario: Attempt to enroll in a non-service tenant
	When I use the tenant store to enroll the tenant called 'Litware' in the service called 'Contoso' anticipating an exception
	Then an 'InvalidMarainTenantTypeException' is thrown

Scenario: Basic enrollment without dependencies or configuration
	When I use the tenant store to enroll the tenant called 'Litware' in the service called 'Simple manifest with no dependencies or configuration'
	Then the tenant called 'Litware' should have the id of the tenant called 'Simple manifest with no dependencies or configuration' added to its enrollments

Scenario: Basic enrollment with configuration
	Given I have enrollment configuration called 'FooBar config'
	And the enrollment configuration called 'FooBar config' contains the following Blob Storage configuration items
	| Key         | Account Name | Container     |
	| fooBarStore | blobaccount  | blobcontainer |
	When I use the tenant store with the enrollment configuration called 'FooBar config' to enroll the tenant called 'Litware' in the service called 'FooBar v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Litware' should contain blob storage configuration under the key 'fooBarStore' for a blob storage container definition with container name 'blobcontainer'

Scenario: Basic enrollment without supplying required configuration
	When I use the tenant store to enroll the tenant called 'Litware' in the service called 'FooBar v1' anticipating an exception
	Then an 'InvalidEnrollmentConfigurationException' is thrown
	Then the tenant called 'Litware' should not have the id of the tenant called 'FooBar v1' in its enrollments

Scenario: Enrollment with a dependency
	# Dependency graph:
	#
	# +---------+         +------------+
	# |         |         |            |
	# | Litware +---------> OPERATIONS +------+
	# |         |         |            |      |
	# +---------+         +------------+      |
	#                                         |
	#                                         |
	#                                   +-----v------+
	#                                   |            |
	#                                   | FOOBAR     |
	#                                   |            |
	#                                   +------------+
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
	#        +-> Operations v1
	#        |     |
	#        |     +-> Operations v1\Litware
	#        |
	#        +-> FooBar
	#
	Given I have enrollment configuration called 'Operations config'
	And the enrollment configuration called 'Operations config' contains the following Blob Storage configuration items
	| Key                                                    | Account Name   | Container        |
	| fooBarStore                                            | fbblobaccount  | fbblobcontainer  |
	| MarainOperations:BlobContainerConfiguration:operations | opsblobaccount | opsblobcontainer |
	When I use the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for a blob storage container definition with container name 'opsblobcontainer'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for a blob storage container definition with container name 'fbblobcontainer'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'

Scenario: Enrollment with a dependency without supplying configuration for the dependency service
	# Dependency graph: as for "Enrollment with a dependency"
	#
	# Expected tenant tree (no changes are expected as the lack of config should mean nothing is modified):
	#
	# Root tenant
	#  |
	#  +-> Client Tenants
	#  |     |
	#  |     +-> Litware
	#  |
	#  +-> Service Tenants
	#        |
	#        +-> Operations v1
	#        |
	#        +-> FooBar
	Given I have enrollment configuration called 'Operations config'
	And the enrollment configuration called 'Operations config' contains the following Blob Storage configuration items
	| Key                                                    | Account Name   | Container        |
	| MarainOperations:BlobContainerConfiguration:operations | opsblobaccount | opsblobcontainer |
	When I use the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1' anticipating an exception
	Then an 'InvalidEnrollmentConfigurationException' is thrown
	Then the tenant called 'Litware' should not have the id of the tenant called 'Operations v1' in its enrollments
	And the tenant called 'Litware' should not contain blob storage configuration under key 'MarainOperations:BlobContainerConfiguration:operations'
	And no new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created

Scenario: Enrollment with multiple levels of dependency
	# Dependency graph:
	#
	# +---------+        +------------+
	# |         |        |            |
	# | Litware +--------> WORKFLOW   +------+
	# |         |        |            |      |
	# +---------+        +------------+      |
	#                                        |
	#                                        |
	#                                  +-----v------+
	#                                  |            |
	#                                  | OPERATIONS +------+
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
	#        +-> Workflow v1
	#        |     |
	#        |     +-> Workflow v1\Litware
	#        |
	#        +-> Operations v1
	#        |     |
	#        |     +-> Operations v1\Workflow v1\Litware
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
	When I use the tenant store with the enrollment configuration called 'Workflow config' to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Workflow v1' added to its enrollments
	And the tenant called 'Litware' should contain Cosmos configuration under the key 'MarainWorkflow:CosmosContainerConfiguration:Definitions' with database name 'wfdb' and container name 'wfcontainer'
	And the tenant called 'Litware' should contain Cosmos configuration under the key 'MarainWorkflow:CosmosContainerConfiguration:Instances' with database name 'wfdb' and container name 'wfinstancecontainer'
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Workflow v1\Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for a blob storage container definition with container name 'opsblobcontainer'
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Workflow v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for a blob storage container definition with container name 'fbblobcontainer'
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1\Workflow v1\Litware' set as the delegated tenant for the service called 'Operations v1'

Scenario: Enrollment with multiple levels of dependency and with the client tenant directly enrolled in one of the dependent services
	# Dependency graph:
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
	When I use the tenant store with the enrollment configuration called 'Workflow config' to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	And I use the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Workflow v1' added to its enrollments
	And the tenant called 'Litware' should contain Cosmos configuration under the key 'MarainWorkflow:CosmosContainerConfiguration:Definitions' with database name 'wfdb' and container name 'wfcontainer'
	And the tenant called 'Litware' should contain Cosmos configuration under the key 'MarainWorkflow:CosmosContainerConfiguration:Instances' with database name 'wfdb' and container name 'wfinstancecontainer'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for a blob storage container definition with container name 'opsblobcontainer2'
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Workflow v1\Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for a blob storage container definition with container name 'opsblobcontainer'
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Workflow v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for a blob storage container definition with container name 'fbblobcontainer'
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1\Workflow v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for a blob storage container definition with container name 'fbblobcontainer2'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	