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

Scenario: Duplicate service names are allowed

Scenario: Well known tenant GUID is already in use
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '085f50fa-5006-4fca-aac1-cf1f74b0198e'
	Given I have a service manifest called 'Operations Manifest 2' for a service called 'Operations v2'
	And the well-known tenant Guid for the manifest called 'Operations Manifest 2' is '085f50fa-5006-4fca-aac1-cf1f74b0198e'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	When I validate the service manifest called 'Operations Manifest 2'
	Then an 'InvalidServiceManifestException' is thrown
	

Scenario: Dependent service does not exist
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Id    |
	| Operations v1 |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown

Scenario Outline: Invalid required configuration items - Blob storage
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following Azure Blob Storage configuration entries
	| Key   | Description   | Container Name   |
	| <Key> | <Description> | <Container Name> |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown
	And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

	Examples:
	| Scenario Description | Key | Description | Container Name | Expected Error Count |
	| Missing key          |     | description | container      | 1                    |
	| Missing description  | key |             | container      | 1                    |
	| Missing container    | key | description |                | 1                    |
	| Missing everything   |     |             |                | 3                    |

Scenario Outline: Invalid required configuration items - CosmosDb storage
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following Azure CosmosDb Storage configuration entries
	| Key   | Description   | Database Name   | Container Name   |
	| <Key> | <Description> | <Database Name> | <Container Name> |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown
	And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

	Examples:
	| Scenario Description | Key | Description | Database Name | Container Name | Expected Error Count |
	| Missing key          |     | description | database      | container      | 1                    |
	| Missing description  | key |             | database      | container      | 1                    |
	| Missing database     | key | description |               | container      | 1                    |
	| Missing container    | key | description | database      |                | 1                    |
	| Missing everything   |     |             |               |                | 4                    |
	