@perScenarioContainer
@useInMemoryTenantProvider

Feature: Adding a new Client tenant
	In order to onboard a new customer
	As an administrator
	I want to create a new client tenant for the customer

Scenario: Create new client tenant
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new client tenant called 'Litware'
	Then there is a tenant called 'Litware' as a child of the tenant called 'Client Tenants'

Scenario: Create new client tenant when the tenancy provider has not been initialised
	When I use the tenant management service to create a new client tenant called 'Litware'
	Then an 'InvalidOperationException' is thrown
