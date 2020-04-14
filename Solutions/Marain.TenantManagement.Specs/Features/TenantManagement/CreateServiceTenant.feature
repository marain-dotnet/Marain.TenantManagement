@perScenarioContainer
@useInMemoryTenantProvider

Feature: Adding a new Service tenant
	In order to enable a Marain service for enrollment
	As an administrator
	I want to create a new service tenant for the service

Background:
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
	And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the well-known tenant Guid for the manifest called 'Workflow Manifest' is '8664b477-3403-4c77-abb2-f9d1b95b0c18'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Name                                                     |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 |
	And I have a service manifest called 'Workflow Manifest with invalid dependency' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest with invalid dependency' has the following dependencies
	| Service Name                                                     |
	| 3633754ac4c9be44b55bfe791b1780f1ac641e0cdeee4246abdf41dfb1e94946 |

Scenario: Create new service tenant
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	Then there is a service tenant with Id '3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858'
	And the tenant with Id '3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858' is called 'Operations v1'

Scenario: Create a new service tenant without supplying a manifest
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant without supplying a manifest
	Then an 'ArgumentNullException' is thrown

Scenario: Create new service tenant when the tenancy provider has not been initialised
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	Then an 'InvalidOperationException' is thrown

Scenario: Create a new service tenant that has a dependency
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	And I use the tenant management service to create a new service tenant with manifest 'Workflow Manifest'
	Then there is a service tenant with Id '3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858'
	And the tenant with Id '3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858' is called 'Operations v1'
	And there is a service tenant with Id '3633754ac4c9be44b55bfe791b1780f177b464860334774cabb2f9d1b95b0c18'
	And the tenant with Id '3633754ac4c9be44b55bfe791b1780f177b464860334774cabb2f9d1b95b0c18' is called 'Workflow v1'

Scenario: Create a new service tenant where at least one of the services in the list of dependencies does not exist
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new service tenant with manifest 'Workflow Manifest with invalid dependency'
	Then an 'InvalidServiceManifestException' is thrown

Scenario: Create a new service tenant where the well-known tenant GUID in the manifest is already in use
	Given the tenancy provider has been initialised for use with Marain
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	When I use the tenant management service to create a new service tenant with manifest 'Operations Manifest'
	Then an 'InvalidServiceManifestException' is thrown

