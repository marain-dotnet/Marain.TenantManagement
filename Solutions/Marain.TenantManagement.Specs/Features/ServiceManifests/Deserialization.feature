@perScenarioContainer

Feature: Service manifest deserialization
	In order to provide a description of a service
	As an engineer
	I want to be able to supply a service manifest as Json

Scenario: Simple manifest with no dependencies or configuration
	When I deserialize the manifest called 'SimpleManifestWithNoDependenciesOrConfiguration'
	Then the resulting manifest should have the service name 'Simple manifest with no dependencies or configuration'
	And the resulting manifest should not have any dependencies
	And the resulting manifest should not have any required configuration entries

Scenario: Manifest with dependencies
	When I deserialize the manifest called 'ManifestWithDependencies'
	Then the resulting manifest should have the service name 'Manifest with dependencies'
	And the resulting manifest should have 2 dependencies
	And the resulting manifest should have a dependency called 'Operations'
	And the resulting manifest should have a dependency called 'FooBar'
	And the resulting manifest should not have any required configuration entries

Scenario: Manifest with a configuration item
	When I deserialize the manifest called 'ManifestWithConfiguration'
	Then the resulting manifest should have the service name 'Manifest with configuration'
	And the resulting manifest should not have any dependencies
	And the resulting manifest should have 1 required configuration entry
	And the configuration item with index 0 should be of type 'ServiceManifestBlobStorageConfigurationEntry'

Scenario: Manifest with multiple configuration items
	When I deserialize the manifest called 'ManifestWithMultipleConfigurationItems'
	Then the resulting manifest should have the service name 'Manifest with multiple configuration items'
	And the resulting manifest should not have any dependencies
	And the resulting manifest should have 2 required configuration entries
	And the configuration item with index 0 should be of type 'ServiceManifestBlobStorageConfigurationEntry'
	And the configuration item with index 1 should be of type 'ServiceManifestCosmosDbConfigurationEntry'

Scenario: Manifest with a configuration item that has an unknown content type