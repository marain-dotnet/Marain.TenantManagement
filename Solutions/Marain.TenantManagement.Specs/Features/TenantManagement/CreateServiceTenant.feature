@perScenarioContainer
@useInMemoryTenantProvider

Feature: Adding a new Service tenant
	In order to enable a Marain service for enrollment
	As an administrator
	I want to create a new service tenant for the service

Background:
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Name |
	| Operations   |

Scenario: Create new service tenant
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	Then there is a tenant called 'Operations v1' as a child of the tenant called 'Service Tenants'

Scenario: Create a new service tenant without supplying a manifest
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant without supplying a manifest
	Then an 'ArgumentNullException' is thrown

Scenario: Create new service tenant when the tenancy provider has not been initialised
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	Then an 'InvalidOperationException' is thrown

Scenario: Create a new service tenant where at least one of the services in the list of dependencies does not exist
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant with manifest 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown

Scenario: Create a new service tenant where the name in the manifest is already in use
	Given the tenancy provider has been initialised for use with Marain
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	Then an 'InvalidServiceManifestException' is thrown

