@perScenarioContainer
@useInMemoryTenantProvider

Feature: Adding a new Client tenant
	In order to onboard a new customer
	As an administrator
	I want to create a new client tenant for the customer

Scenario: Create new client tenant
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant store to create a new client tenant called 'Litware'
	Then there is a client tenant called 'Litware'

Scenario: Create new client tenant when the tenancy provider has not been initialised
	When I use the tenant store to create a new client tenant called 'Litware'
	Then an 'InvalidOperationException' is thrown

Scenario: Create a new client tenant with a well known GUID
	Given the tenancy provider has been initialised for use with Marain
	When I use the tenant store to create a new client tenant with well known Guid 'd620a845-de25-4b88-b5a1-27d6b1104f82' called 'Litware'
	Then the tenant with Id '75b9261673c2714681f14c97bc0439fb45a820d625de884bb5a127d6b1104f82' is called 'Litware'

Scenario: Create a new child client tenant of a client tenant with a well known GUID 
	Given the tenancy provider has been initialised for use with Marain
	And I have an existing client tenant with a well known Guid '102fdab0-6d9c-4f99-bf87-ce94ad3bbae6' called 'Fabrikam'
	When I use the tenant store to create a new child client tenant of the 'Fabrikam' client tenant with well known Guid 'd620a845-de25-4b88-b5a1-27d6b1104f82' called 'Litware'
	Then the tenant with Id '75b9261673c2714681f14c97bc0439fbb0da2f109c6d994fbf87ce94ad3bbae645a820d625de884bb5a127d6b1104f82' is called 'Litware'
	And the tenant with Id '75b9261673c2714681f14c97bc0439fbb0da2f109c6d994fbf87ce94ad3bbae645a820d625de884bb5a127d6b1104f82' has a parent with Id '75b9261673c2714681f14c97bc0439fbb0da2f109c6d994fbf87ce94ad3bbae6'

Scenario: Create a new child client tenant of a client tenant which does not exist 
	Given the tenancy provider has been initialised for use with Marain
	And the client tenant 'Fabrikam' does not exist
	When I use the tenant store to create a new child client tenant of the 'Fabrikam' client tenant with well known Guid 'd620a845-de25-4b88-b5a1-27d6b1104f82' called 'Litware'
	Then a 'TenantNotFoundException' is thrown

Scenario: Create a new child client tenant of a service tenant with a well known GUID
	Given I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And the well-known tenant Guid for the manifest called 'Operations Manifest' is '4f522924-b6e7-48cc-a265-a307407ec858'
	And the tenancy provider has been initialised for use with Marain
	And I have an existing service tenant with manifest 'Operations Manifest'
	When I use the tenant store to create a new child client tenant of the 'Operations v1' client tenant with well known Guid 'd620a845-de25-4b88-b5a1-27d6b1104f82' called 'Litware'
	Then an 'InvalidMarainTenantTypeException' is thrown