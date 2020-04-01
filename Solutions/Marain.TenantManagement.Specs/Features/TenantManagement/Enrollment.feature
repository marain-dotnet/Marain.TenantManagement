@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enrollment
	In order to allow a client to use a Marain service
	As an administrator
	I want to enroll a tenant to use that service

Background:
	Given the tenancy provider has been initialised for use with Marain
	And I have a service manifest called 'Operations Manifest' for a service called 'Operations v1'
	And I have used the tenant management service to create a service tenant with manifest 'Operations Manifest'
	And I have used the tenant management service to create a new client tenant called 'Litware'

Scenario: Basic enrollment without dependencies or configuration
	When I use the tenant enrollment service to enroll the tenant called 'Litware' in the service called 'Operations v1'
	Then the tenant called 'Litware' should have the id of the tenant called 'Operations v1' added to its enrollments
