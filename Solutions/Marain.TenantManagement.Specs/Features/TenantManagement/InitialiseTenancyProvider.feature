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
	And there is a tenant with Id '75b9261673c2714681f14c97bc0439fb' as a child of the root tenant
	And the tenant with Id '75b9261673c2714681f14c97bc0439fb' is called 'Client Tenants'
	And there is a tenant with Id '3633754ac4c9be44b55bfe791b1780f1' as a child of the root tenant
	And the tenant with Id '3633754ac4c9be44b55bfe791b1780f1' is called 'Service Tenants'

Scenario: Initialise a tenancy provider that's already been initialised
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to initialise the tenancy provider
	Then the tenancy provider contains 2 tenants as children of the root tenant

Scenario: Initialise a non-empty but uninitialised tenancy provider
	Given the tenancy provider contains 5 tenants as children of the root tenant
	When I use the tenant management service to initialise the tenancy provider
	Then an 'InvalidOperationException' is thrown
	And the tenancy provider contains 5 tenants as children of the root tenant
	And there is no tenant with Id '75b9261673c2714681f14c97bc0439fb' as a child of the root tenant
	And there is no tenant with Id '3633754ac4c9be44b55bfe791b1780f1' as a child of the root tenant

Scenario: Initialise a non-empty but uninitialised tenancy provider with the force option
	Given the tenancy provider contains 5 tenants as children of the root tenant
	When I use the tenant management service to initialise the tenancy provider using the force option
	Then the tenancy provider contains 7 tenants as children of the root tenant
	And there is a tenant with Id '75b9261673c2714681f14c97bc0439fb' as a child of the root tenant
	And there is a tenant with Id '3633754ac4c9be44b55bfe791b1780f1' as a child of the root tenant
