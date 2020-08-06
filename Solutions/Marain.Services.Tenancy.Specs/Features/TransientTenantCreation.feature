@perFeatureContainer

Feature: Transient tenant creation
	In order to test my Marain services
	As a developer
	I want to be able to create transient tenants

Scenario: Create transient service tenant from embedded resource
	When I create a transient service tenant using the embedded resource 'Marain.Services.Tenancy.Specs.Data.ServiceManifests.OperationsServiceManifest.jsonc'
	Then the transient tenant is created
	And the transient tenant Id is different from the service as described in the manifest

Scenario: Create transient client tenant
	When I create a transient client tenant
	Then the transient tenant is created