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
    | Key   | Description   |
    | <Key> | <Description> |
    When I validate the service manifest called 'Workflow Manifest'
    Then an 'InvalidServiceManifestException' is thrown
    And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

    Examples:
    | Scenario Description | Key | Description | Expected Error Count |
    | Missing key          |     | description | 1                    |
    | Missing description  | key |             | 1                    |
    | Missing everything   |     |             | 2                    |

Scenario Outline: Invalid required configuration items - Table storage
    Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following legacy V2 Azure Table Storage configuration entries
    | Key   | Description   |
    | <Key> | <Description> |
    When I validate the service manifest called 'Workflow Manifest'
    Then an 'InvalidServiceManifestException' is thrown
    And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

    Examples:
    | Scenario Description | Key | Description | Expected Error Count |
    | Missing key          |     | description | 1                    |
    | Missing description  | key |             | 1                    |
    | Missing everything   |     |             | 2                    |

Scenario Outline: Invalid required configuration items - CosmosDb storage
    Given I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following legacy V2 Azure CosmosDb Storage configuration entries
    | Key   | Description   |
    | <Key> | <Description> |
    When I validate the service manifest called 'Workflow Manifest'
    Then an 'InvalidServiceManifestException' is thrown
    And the list of errors attached to the InvalidServiceManifestException contains <Expected Error Count> entries

    Examples:
    | Scenario Description | Key | Description | Expected Error Count |
    | Missing key          |     | description | 1                    |
    | Missing description  | key |             | 1                    |
    | Missing everything   |     |             | 2                    |
    