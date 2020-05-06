@perScenarioContainer
@useInMemoryTenantProvider

Feature: Adding a new Client tenant
	In order to onboard a new customer
	As an administrator
	I want to create a new client tenant for the customer

Scenario: Create new client tenant
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new client tenant called 'Litware'
	Then there is a client tenant called 'Litware'

Scenario: Create new client tenant when the tenancy provider has not been initialised
	When I use the tenant management service to create a new client tenant called 'Litware'
	Then an 'InvalidOperationException' is thrown

Scenario: Create a new client tenant with a well known GUID
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant management service to create a new client tenant with well known Guid 'd620a845-de25-4b88-b5a1-27d6b1104f82' called 'Litware'
	Then the tenant with Id '75b9261673c2714681f14c97bc0439fb45a820d625de884bb5a127d6b1104f82' is called 'Litware'
