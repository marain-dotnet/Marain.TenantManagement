@perScenarioContainer
@useInMemoryTenantProvider

Feature: Service manifest dependent service validation
    In order to maintain the integrity of the system
    As a developer
    I want to be able to validate Service Manifests with dependencies

Background: 
    Given the tenancy provider has been initialised for use with Marain

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
    And I have used the tenant store to create a service tenant with manifest 'Operations Manifest'
    And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following dependencies
    | Service Id                                                       | Expected Name |
    | 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 |               |
    When I validate the service manifest called 'Workflow Manifest'
    Then no exception is thrown

Scenario: Dependent service with an expected name that matches the service name is valid
    Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
    And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
    And I have used the tenant store to create a service tenant with manifest 'Operations Manifest'
    And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following dependencies
    | Service Id                                                       | Expected Name |
    | 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v1 |
    When I validate the service manifest called 'Workflow Manifest'
    Then no exception is thrown

Scenario: Dependent service with an expected name that does not match the service name is invalid
    Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
    And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
    And I have used the tenant store to create a service tenant with manifest 'Operations Manifest'
    And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following dependencies
    | Service Id                                                       | Expected Name |
    | 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v2 |
    When I validate the service manifest called 'Workflow Manifest'
    Then an 'InvalidServiceManifestException' is thrown

Scenario: Duplicate Ids in list of dependencies (regardless of expected names)
    Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
    And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
    And I have used the tenant store to create a service tenant with manifest 'Operations Manifest'
    And I have a service manifest called 'Workflow Manifest' for a service called 'Workflow v1'
    And the service manifest called 'Workflow Manifest' has the following dependencies
    | Service Id                                                       | Expected Name |
    | 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v1 |
    | 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 |               |
    | 3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858 | Operations v1 |
    When I validate the service manifest called 'Workflow Manifest'
    Then an 'InvalidServiceManifestException' is thrown
