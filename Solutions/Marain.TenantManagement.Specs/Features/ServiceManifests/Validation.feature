@perScenarioContainer
@useInMemoryTenantProvider

Feature: Service manifest validation
	In order to maintain the integrity of the system
	As a developer
	I want to be able to validate Service Manifests

Background: 
	Given the tenancy provider has been initialised for use with Marain

Scenario: Manifest name not set
	Given I have a service manifest called 'Manifest' with no service name
	When I validate the service manifest called 'Manifest'
	Then an 'InvalidServiceManifestException' is thrown
	
Scenario Outline: Creating a manifest with invalid service names
	Given I have a service manifest called 'Manifest' for a service called '<Service Name>'
	When I validate the service manifest called 'Manifest'
	Then an 'InvalidServiceManifestException' is thrown

	Examples:
		| Scenario Description | Service Name |
		| Empty string         | ""           |
		| Spaces only          | "  "         |
		| Tabs only            | "	"         |

Scenario: Manifest name already in use
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	When I validate the service manifest called 'Operations Manifest'
	Then an 'InvalidServiceManifestException' is thrown

Scenario: Dependent service does not exist
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Name  |
	| Operations v1 |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown
