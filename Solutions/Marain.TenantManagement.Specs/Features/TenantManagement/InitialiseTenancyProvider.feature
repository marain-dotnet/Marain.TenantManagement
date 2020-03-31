@perScenarioContainer
@useInMemoryTenantProvider

Feature: Tenancy provider initialisation
	When setting up a new Marain instance
	As an administrator
	I want to initialise the tenancy provider with standard Marain entries

Scenario: Initialise an empty tenancy provider
	Given No tenants have been created
	When I use the tenant management service to initialise the tenancy provider
	Then the tenancy provider contains 2 tenants as children of the root tenant
	And there is a tenant called 'Client Tenants' as a child of the root tenant
	And there is a tenant called 'Service Tenants' as a child of the root tenant

Scenario: Initialise a tenancy provider that's already been initialised
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to initialise the tenancy provider
	Then the tenancy provider contains 2 tenants as children of the root tenant

Scenario: Initialise a non-empty tenancy provider
	Given the tenancy provider contains 5 tenants as children of the root tenant
	When I use the tenant management service to initialise the tenancy provider
	Then an 'InvalidOperationException' is thrown
	And the tenancy provider contains 5 tenants as children of the root tenant
	And there is no tenant called 'Client Tenants' as a child of the root tenant
	And there is no tenant called 'Service Tenants' as a child of the root tenant

Scenario: Initialise a non-empty tenancy provider with the force option
	Given the tenancy provider contains 5 tenants as children of the root tenant
	When I use the tenant management service to initialise the tenancy provider using the force option
	Then the tenancy provider contains 7 tenants as children of the root tenant
	And there is a tenant called 'Client Tenants' as a child of the root tenant
	And there is a tenant called 'Service Tenants' as a child of the root tenant
