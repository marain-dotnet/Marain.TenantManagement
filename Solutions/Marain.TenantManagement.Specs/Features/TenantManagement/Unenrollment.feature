@perScenarioContainer
@useInMemoryTenantProvider

Feature: Unenrollment
	In order to disallow a currently allowed a client from using a Marain service
	As an administrator
	I want to unenroll a tenant from a service

Background:
	Given the tenancy provider has been initialised for use with Marain
	And I have loaded the manifest called 'SimpleManifestWithNoDependenciesOrConfiguration'
	And I have used the tenant management service to create a service tenant with manifest 'SimpleManifestWithNoDependenciesOrConfiguration'
	And I have loaded the manifest called 'FooBarServiceManifest'
	And I have used the tenant management service to create a service tenant with manifest 'FooBarServiceManifest'
	And I have loaded the manifest called 'OperationsServiceManifest'
	And I have used the tenant management service to create a service tenant with manifest 'OperationsServiceManifest'
	And I have loaded the manifest called 'WorkflowServiceManifest'
	And I have used the tenant management service to create a service tenant with manifest 'WorkflowServiceManifest'
	And I have used the tenant management service to create a new client tenant called 'Litware'
	And I have used the tenant management service to create a new client tenant called 'Contoso'

Scenario: Unenroll from a service that the client has not previously enrolled in
	When I use the tenant management service to unenroll the tenant called 'Litware' from the service called 'Simple manifest with no dependencies or configuration'
	Then an 'InvalidOperationException' is thrown

Scenario: Basic unenrollment with no dependencies or configuration
	Given I have used the tenant management service to enroll the tenant called 'Litware' in the service called 'Simple manifest with no dependencies or configuration'
	When I use the tenant management service to unenroll the tenant called 'Litware' from the service called 'Simple manifest with no dependencies or configuration'
	Then the tenant called 'Litware' should not have the id of the tenant called 'Simple manifest with no dependencies or configuration' in its enrollments

Scenario: Basic unenrollment with configuration
	Given I have enrollment configuration called 'FooBar config'
	And the enrollment configuration called 'FooBar config' contains the following Blob Storage configuration items
	| Key         | Account Name | Container     |
	| fooBarStore | blobaccount  | blobcontainer |
	And I have used the tenant management service with the enrollment configuration called 'FooBar config' to enroll the tenant called 'Litware' in the service called 'FooBar v1'
	When I use the tenant management service to unenroll the tenant called 'Litware' from the service called 'FooBar v1'
	Then the tenant called 'Litware' should not have the id of the tenant called 'FooBar v1' in its enrollments
	And the tenant called 'Litware' should not contain blob storage configuration for a blob storage container definition with container name 'foobar'

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
	| Key             | Account Name   | Container        |
	| fooBarStore     | fbblobaccount  | fbblobcontainer  |
	| operationsStore | opsblobaccount | opsblobcontainer |
	And the enrollment configuration called 'Workflow config' contains the following Cosmos configuration items
	| Key                   | Account Uri | Database Name | Container Name      |
	| workflowStore         | wfaccount   | wfdb          | wfcontainer         |
	| workflowInstanceStore | wfaccount   | wfdb          | wfinstancecontainer |
	And I have enrollment configuration called 'Operations config'
	And the enrollment configuration called 'Operations config' contains the following Blob Storage configuration items
	| Key             | Account Name    | Container         |
	| fooBarStore     | fbblobaccount2  | fbblobcontainer2  |
	| operationsStore | opsblobaccount2 | opsblobcontainer2 |
	And I have used the tenant management service with the enrollment configuration called 'Workflow config' to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	And I have used the tenant management service with the enrollment configuration called 'Operations config' to enroll the tenant called 'Litware' in the service called 'Operations v1'
	When I use the tenant management service to unenroll the tenant called 'Litware' from the service called 'Workflow v1'
	Then the tenant called 'Litware' should not have the id of the tenant called 'Workflow v1' in its enrollments
	And the tenant called 'Litware' should not contain Cosmos configuration for a Cosmos container definition with database name 'workflow' and container name 'definitions'
	And the tenant called 'Litware' should not contain Cosmos configuration for a Cosmos container definition with database name 'workflow' and container name 'instances'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should contain blob storage configuration for a blob storage container definition with container name 'operations'
	And there should not be a child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1'
	And the tenant called 'Litware' should not have a delegated tenant for the service called 'Workflow v1'
	And there should not be a child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Operations v1\Litware' should contain blob storage configuration for a blob storage container definition with container name 'foobar'
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'