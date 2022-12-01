@perScenarioContainer

Feature: Service manifest deserialization
    In order to provide a description of a service
    As an engineer
    I want to be able to supply a service manifest as Json

Scenario: Simple manifest with no dependencies or configuration
    When I deserialize the manifest called 'ServiceManifestC0D()'
    Then the resulting manifest should have the service name 'SvcC0D()'
    And the resulting manifest should have a well known service GUID of 'b4dd52e4-a6cd-4e7b-842f-059528a23153'
    And the resulting manifest should not have any dependencies
    And the resulting manifest should not have any required configuration entries

Scenario: Manifest with dependencies
    When I deserialize the manifest called 'ManifestWithDependencies'
    Then the resulting manifest should have the service name 'Manifest with dependencies'
    And the resulting manifest should have a well known service GUID of '31b74bd9-b45d-4402-81cf-13fc6f680a85'
    And the resulting manifest should have 2 dependencies
    And the resulting manifest should have a dependency with Id '3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858'
    And the resulting manifest should have a dependency with Id '3633754ac4c9be44b55bfe791b1780f187732ecba95e0e41b5ad8b2605aa5fb3'
    And the resulting manifest should not have any required configuration entries

Scenario: Manifest with a configuration item
    When I deserialize the manifest called 'ManifestWithConfiguration'
    Then the resulting manifest should have the service name 'Manifest with configuration'
    And the resulting manifest should have a well known service GUID of '910825dd-7aa1-44f1-9fbf-c95e8b43e7e2'
    And the resulting manifest should not have any dependencies
    And the resulting manifest should have 1 required configuration entry
    And the configuration item with index 0 should be of type 'ServiceManifestBlobStorageConfigurationEntry'

Scenario: Manifest with multiple configuration items
    When I deserialize the manifest called 'ManifestWithMultipleConfigurationItems'
    Then the resulting manifest should have the service name 'Manifest with multiple configuration items'
    And the resulting manifest should have a well known service GUID of 'ba80b530-20c6-4c78-ab18-8ac70cb4db9e'
    And the resulting manifest should not have any dependencies
    And the resulting manifest should have 3 required configuration entries
    And the configuration item with index 0 should be of type 'ServiceManifestBlobStorageConfigurationEntry'
    And the ServiceManifestBlobStorageConfigurationEntry at index 0 should have a null LegacyV2Key
    And the configuration item with index 1 should be of type 'ServiceManifestCosmosDbConfigurationEntry'
    And the ServiceManifestCosmosDbConfigurationEntry at index 1 should have a null LegacyV2Key
    And the configuration item with index 2 should be of type 'ServiceManifestTableStorageConfigurationEntry'
    And the ServiceManifestTableStorageConfigurationEntry at index 2 should have a null LegacyV2Key

Scenario: Manifest with a configuration item that has an unknown content type
    When I deserialize the manifest called 'ManifestWithConfigurationWithUnknownContentType' anticipating an exception
    Then an 'InvalidOperationException' is thrown

Scenario: Manifest with multiple V2 to V3 migration configuration items
    When I deserialize the manifest called 'LegacyV2.SvcC3V3orV2D()'
    Then the resulting manifest should have the service name 'SvcC3V3orV2D()'
    And the resulting manifest should have a well known service GUID of '19f30a02-527d-4abf-ac35-aa29abc985cc'
    And the resulting manifest should not have any dependencies
    And the resulting manifest should have 3 required configuration entries
    And the configuration item with index 0 should be of type 'ServiceManifestBlobStorageConfigurationEntry'
    And the ServiceManifestBlobStorageConfigurationEntry at index 0 should have a LegacyV2Key of 'StorageConfiguration__mycontainer'
    And the configuration item with index 1 should be of type 'ServiceManifestCosmosDbConfigurationEntry'
    And the ServiceManifestCosmosDbConfigurationEntry at index 1 should have a LegacyV2Key of 'StorageConfiguration__mydb__mycontainer'
    And the configuration item with index 2 should be of type 'ServiceManifestTableStorageConfigurationEntry'
    And the ServiceManifestTableStorageConfigurationEntry at index 2 should have a LegacyV2Key of 'StorageConfiguration__mytable'
