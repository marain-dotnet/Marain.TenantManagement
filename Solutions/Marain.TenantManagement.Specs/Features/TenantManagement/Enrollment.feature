@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enrollment
	In order to allow a client to use a Marain service
	As an administrator
	I want to enroll a tenant to use that service

Background:
	Given the tenancy provider has been initialised for use with Marain
	And I have a service manifest called 'FooBar Manifest' for a service called 'FooBar v1'
	And I have used the tenant management service to create a service tenant with manifest 'FooBar Manifest'
	And I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the service manifest called 'Operations Manifest' has the following dependencies
	| Service Name |
	| FooBar v1    |
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Name  |
	| Operations v1 |
	And I have used the tenant management service to create a service tenant with manifest 'Workflow Manifest'
	And I have used the tenant management service to create a new client tenant called 'Litware'

Scenario: Basic enrollment without dependencies or configuration
	When I use the tenant enrollment service to enroll the tenant called 'Litware' in the service called 'FooBar v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments

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
	When I use the tenant enrollment service to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'

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
	When I use the tenant enrollment service to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Workflow v1' added to its enrollments
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
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
	When I use the tenant enrollment service to enroll the tenant called 'Litware' in the service called 'Workflow v1'
	And I use the tenant enrollment service to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Workflow v1' added to its enrollments
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And a new child tenant called 'Workflow v1\Litware' of the service tenant called 'Workflow v1' has been created
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
	And the tenant called 'Litware' should have the id of the tenant called 'Workflow v1\Litware' set as the delegated tenant for the service called 'Workflow v1'
	And a new child tenant called 'Operations v1\Workflow v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Workflow v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Workflow v1\Litware' should have the id of the tenant called 'Operations v1\Workflow v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	And a new child tenant called 'Operations v1\Litware' of the service tenant called 'Operations v1' has been created
	And the tenant called 'Operations v1\Litware' should have the id of the tenant called 'FooBar v1' added to its enrollments
	And the tenant called 'Litware' should have the id of the tenant called 'Operations v1\Litware' set as the delegated tenant for the service called 'Operations v1'
	