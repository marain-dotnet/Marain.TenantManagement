@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enrollment
	In order to allow a client to use a Marain service
	As an administrator
	I want to enroll a tenant to use that service

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
	When I use the tenant store to enroll the tenant called 'Litware' in the service called 'Contoso'
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
	And the tenant called 'Litware' should contain blob storage configuration for a blob storage container definition with container name 'foobar'

Scenario: Basic enrollment without supplying required configuration
	When I use the tenant store to enroll the tenant called 'Litware' in the service called 'FooBar v1'
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
	| Key             | Account Name   | Container        |
	| fooBarStore     | fbblobaccount  | fbblobcontainer  |
	| operationsStore | opsblobaccount | opsblobcontainer |
	When I use the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should contain blob storage configuration for a blob storage container definition with container name 'operations'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'foobar'
	And the tenant called 'Operations v1\Litware` should have its on-behalf-of tenant Id set to the id of the tenant called 'Litware'
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
	| Key             | Account Name   | Container        |
	| operationsStore | opsblobaccount | opsblobcontainer |
	When I use the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then an 'InvalidEnrollmentConfigurationException' is thrown
	Then the tenant called 'Litware' should not have the id of the tenant called 'Operations v1' in its enrollments
	And the tenant called 'Litware' should not contain blob storage configuration for a blob storage container definition with container name 'operations'
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
	| Key             | Account Name   | Container        |
	| fooBarStore     | fbblobaccount  | fbblobcontainer  |
	| operationsStore | opsblobaccount | opsblobcontainer |
	And the enrollment configuration called 'Workflow config' contains the following Cosmos configuration items
	| Key                   | Account Uri | Database Name | Container Name      |
	| workflowStore         | wfaccount   | wfdb          | wfcontainer         |
	| workflowInstanceStore | wfaccount   | wfdb          | wfinstancecontainer |
	And the enrollment configuration called 'Workflow config' contains the following Table Storage configuration items
	| Key        | Account Name   | Table   |
	| auditStore | fbtableaccount | fbtable |
	When I use the tenant store with the enrollment configuration called 'Workflow config' to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Workflow v1' added to its enrollments
	And the tenant called 'Litware' should contain Cosmos configuration for a Cosmos container definition with database name 'workflow' and container name 'definitions'
	And the tenant called 'Litware' should contain Cosmos configuration for a Cosmos container definition with database name 'workflow' and container name 'instances'
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Workflow v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'operations'
	And the tenant called 'Workflow v1\Litware` should have its on-behalf-of tenant Id set to the id of the tenant called 'Litware'
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Workflow v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'foobar'
	And the tenant called 'Operations v1\Workflow v1\Litware` should have its on-behalf-of tenant Id set to the id of the tenant called 'Workflow v1\Litware'
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
	| Key             | Account Name   | Container        |
	| fooBarStore     | fbblobaccount  | fbblobcontainer  |
	| operationsStore | opsblobaccount | opsblobcontainer |
	And the enrollment configuration called 'Workflow config' contains the following Cosmos configuration items
	| Key                   | Account Uri | Database Name | Container Name      |
	| workflowStore         | wfaccount   | wfdb          | wfcontainer         |
	| workflowInstanceStore | wfaccount   | wfdb          | wfinstancecontainer |
	And the enrollment configuration called 'Workflow config' contains the following Table Storage configuration items
	| Key        | Account Name   | Table   |
	| auditStore | fbtableaccount | fbtable |
	And I have enrollment configuration called 'Operations config'
	And the enrollment configuration called 'Operations config' contains the following Blob Storage configuration items
	| Key             | Account Name    | Container         |
	| fooBarStore     | fbblobaccount2  | fbblobcontainer2  |
	| operationsStore | opsblobaccount2 | opsblobcontainer2 |
	When I use the tenant store with the enrollment configuration called 'Workflow config' to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	And I use the tenant store with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Workflow v1' added to its enrollments
	And the tenant called 'Litware' should contain Cosmos configuration for a Cosmos container definition with database name 'workflow' and container name 'definitions'
	And the tenant called 'Litware' should contain Cosmos configuration for a Cosmos container definition with database name 'workflow' and container name 'instances'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should contain blob storage configuration for a blob storage container definition with container name 'operations'
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Workflow v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'operations'
	And the tenant called 'Workflow v1\Litware` should have its on-behalf-of tenant Id set to the id of the tenant called 'Litware'
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Workflow v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'foobar'
	And the tenant called 'Operations v1\Workflow v1\Litware` should have its on-behalf-of tenant Id set to the id of the tenant called 'Workflow v1\Litware'
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1\Workflow v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'foobar'
	And the tenant called 'Operations v1\Litware` should have its on-behalf-of tenant Id set to the id of the tenant called 'Litware'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	