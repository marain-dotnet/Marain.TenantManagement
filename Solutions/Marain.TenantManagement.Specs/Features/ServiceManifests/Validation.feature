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
	
Scenario: Missing well-known tenant GUID
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '00000000-0000-0000-0000-000000000000'
	When I validate the service manifest called 'Operations Manifest'
	Then an 'InvalidServiceManifestException' is thrown

Scenario: Well known tenant GUID is already in use
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '085f50fa-5006-4fca-aac1-cf1f74b0198e'
	Given I have a service manifest called 'Operations Manifest 2' for a service called 'Operations v2'
	And the well-known tenant Guid for the manifest called 'Operations Manifest 2' is '085f50fa-5006-4fca-aac1-cf1f74b0198e'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	When I validate the service manifest called 'Operations Manifest 2'
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

Scenario: Duplicate service names are allowed as long as the well known tenant GUIDs are unique
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations'
	Given I have a service manifest called 'Operations Manifest 2' for a service called 'Operations'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	When I validate the service manifest called 'Operations Manifest 2'
	Then no exception is thrown

Scenario: Dependent service does not exist
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Id                                                       | Expected Name |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 |               |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown

Scenario: Dependent service without an expected name is valid
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Id                                                       | Expected Name |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 |               |
	When I validate the service manifest called 'Workflow Manifest'
	Then no exception is thrown

Scenario: Dependent service with an expected name that matches the service name is valid
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Id                                                       | Expected Name |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v1 |
	When I validate the service manifest called 'Workflow Manifest'
	Then no exception is thrown

Scenario: Dependent service with an expected name that does not match the service name is invalid
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Id                                                       | Expected Name |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v2 |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown

Scenario: Duplicate Ids in list of dependencies (regardless of expected names)
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following dependencies
	| Service Id                                                       | Expected Name |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v1 |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 |               |
	| 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v1 |
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

Scenario Outline: Invalid required configuration items - Table storage
	Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
	And the service manifest called 'Workflow Manifest' has the following Azure Table Storage configuration entries
	| Key   | Description   | Table Name   |
	| <Key> | <Description> | <Table Name> |
	When I validate the service manifest called 'Workflow Manifest'
	Then an 'InvalidServiceManifestException' is thrown
	And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

	Examples:
	| Scenario Description | Key | Description | Table Name | Expected Error Count |
	| Missing key          |     | description | table          | 1                    |
	| Missing description  | key |             | table          | 1                    |
	| Missing table        | key | description |                | 1                    |
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
	