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
    And I have used the tenant store to create a service tenant with manifest 'Operations Manifest'
    When I validate the service manifest called 'Operations Manifest 2'
    Then an 'InvalidServiceManifestException' is thrown

Scenario: Well known tenant GUID is already in use by legacy V2 manifest
    Given I have a legacy V2 service manifest called 'Operations Manifest' for a service called 'Operations v1'
    And the well-known tenant Guid for the legacy V2 manifest called 'Operations Manifest' is '085f50fa-5006-4fca-aac1-cf1f74b0198e'
    Given I have a service manifest called 'Operations Manifest 2' for a service called 'Operations v2'
    And the well-known tenant Guid for the manifest called 'Operations Manifest 2' is '085f50fa-5006-4fca-aac1-cf1f74b0198e'
    And I have used the tenant store to create a service tenant with legacy V2 manifest 'Operations Manifest'
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
    And I have used the tenant store to create a service tenant with manifest 'Operations Manifest'
    When I validate the service manifest called 'Operations Manifest 2'
    Then no exception is thrown
