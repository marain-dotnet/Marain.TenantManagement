@perScenarioContainer
@useInMemoryTenantProvider

Feature: Attempt to Enroll Service Without Supplying Required V3 Configuration
    In order to allow a client to use a Marain service that requires V3 configuration
    As an administrator
    I want Marain to detect when I have failed to supply the necessary configuration

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC1D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D()'
    And I have loaded the manifest called 'ServiceManifestC0D(C1D())'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC0D(C1D())'
    And I have used the tenant store to create a new client tenant called 'Litware'


# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | Service with    |
# | Litware +---------| 1 Configuration |
# |         |         | 0 Dependencies  |
# +---------+         +-----------------+
Scenario: Basic enrollment without supplying required configuration
    When I use the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC1D()' anticipating an exception
    Then an 'InvalidEnrollmentConfigurationException' is thrown
    Then the tenant called 'Litware' should not have the id of the tenant called 'SvcC1D()' in its enrollments

# Dependency graph:
#
# +---------+         +------------------+
# |         |         | Service with     |
# | Litware +---------> 0 Configurations +-----+
# |         |         | 1 Dependency     |     |
# +---------+         +------------------+     |
#                                              |
#                                              |
#                                     +--------v--------+
#                                     | Service with    |
#                                     | 1 Configuration |
#                                     | 0 Dependencies  |
#                                     +-----------------+
#
# Expected tenant tree:
#
# Root tenant
#  |
#  +-> Client Tenants
#  |     |
#  |     +-> Litware
#  |
#  +-> Service Tenants
#        |
#        +-> SvcC0D(C1D())
#        |     |
#        |     +-> SvcC0D(C1D())\Litware
#        |
#        +-> SvcC1D()
#
Scenario: Enrollment of a service with a dependency requiring configuration without supplying configuration for the dependency service
    Given I have enrollment configuration called 'Config'
    When I use the tenant store to enroll the tenant called 'Litware' in the service called 'SvcC0D(C1D())' anticipating an exception
	Then an 'InvalidEnrollmentConfigurationException' is thrown
    And the tenant called 'Litware' should not have the id of the tenant called 'SvcC0D(C1D())' in its enrollments
    And no new child tenant called 'SvcC0D(C1D())\Litware' of the service tenant called 'SvcC0D(C1D())' has been created


# TODO: service requiring multiple config, and only some supplied
