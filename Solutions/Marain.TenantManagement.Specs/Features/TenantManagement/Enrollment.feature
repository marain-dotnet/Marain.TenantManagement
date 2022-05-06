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
	And the tenant called 'Workflow v1\Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for the account 'opsblobaccount' and container name 'opsblobcontainer'
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
	And the tenant called 'Operations v1\Workflow v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
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
	And the tenant called 'Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for the account 'opsblobaccount2' and container name 'opsblobcontainer2'
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Workflow v1\Litware' should contain blob storage configuration under the key 'MarainOperations:BlobContainerConfiguration:operations' for a blob storage container definition with container name 'opsblobcontainer'
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
	And the tenant called 'Operations v1\Workflow v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for the account 'fbblobaccount' and container name 'fbblobcontainer'
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1\Workflow v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'SvcC1D()' added to its enrollments
	And the tenant called 'Operations v1\Litware' should contain blob storage configuration under the key 'fooBarStore' for the account 'fbblobaccount2' and container name 'fbblobcontainer2'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	