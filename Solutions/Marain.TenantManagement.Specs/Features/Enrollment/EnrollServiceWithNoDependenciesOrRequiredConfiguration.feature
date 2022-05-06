@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service with No Dependencies and No Required Configuration
    In order to allow a client to use a simple Marain service with no configuration requirements or dependencies
    As an administrator
    I want to enroll a tenant to use that service

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC0D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D()'
    And I have used the tenant store to create a new client tenant called 'Litware'
    And I have used the tenant store to create a new client tenant called 'Contoso'

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------| 0 Configurations |
# |         |         | 0 Dependencies   |
# +---------+         +------------------+
Scenario: Basic enrollment without dependencies or configuration
    When I use the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC0D()'
    Then the tenant called 'Litware' should have the id of the tenant called 'SvcC0D()' added to its enrollments

Scenario: Attempt to enroll in a non-service tenant
    When I use the tenant store to enroll the tenant called 'Litware' in the service called 'Contoso' anticipating an exception
    Then an 'InvalidMarainTenantTypeException' is thrown

