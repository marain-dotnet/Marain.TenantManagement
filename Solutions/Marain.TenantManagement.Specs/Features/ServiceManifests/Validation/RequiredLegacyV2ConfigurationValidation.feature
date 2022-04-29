@perScenarioContainer
@useInMemoryTenantProvider

Feature: Service manifest legacy V2 required configuration validation
    In order to maintain the integrity of the system
    As a developer
    I want to be able to validate Service Manifests that use V3 manifests but with required legacy V2 configuration

Background: 
    Given the tenancy provider has been initialised for use with Marain

Scenario Outline: Invalid required configuration items - Blob storage
    Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following legacy V2 Azure Blob Storage configuration entries
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
    And the service manifest called 'Workflow Manifest' has the following legacy V2 Azure Table Storage configuration entries
    | Key   | Description   | Table Name   |
    | <Key> | <Description> | <Table Name> |
    When I validate the service manifest called 'Workflow Manifest'
    Then an 'InvalidServiceManifestException' is thrown
    And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

    Examples:
    | Scenario Description | Key | Description | Table Name | Expected Error Count |
    | Missing key          |     | description | table      | 1                    |
    | Missing description  | key |             | table      | 1                    |
    | Missing table        | key | description |            | 1                    |
    | Missing everything   |     |             |            | 3                    |

Scenario Outline: Invalid required configuration items - CosmosDb storage
    Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following legacy V2 Azure CosmosDb Storage configuration entries
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
    