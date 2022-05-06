@perScenarioContainer
@useInMemoryTenantProvider

Feature: Enroll Service Requiring V3 Configuration Supplying Incorrect Configuration
    In order to allow a client to use a Marain service that requires V3 configuration
    As an administrator
    I want Marain to detect when I have failed to supply the incorrect configuration

Background:
    Given the tenancy provider has been initialised for use with Marain
    And I have loaded the manifest called 'ServiceManifestC1D()'
    And I have used the tenant store to create a service tenant with manifest 'ServiceManifestC1D()'
    And I have used the tenant store to create a new client tenant called 'Litware'

# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | Service with    |
# | Litware +---------> 1 Configuration |
# |         |         | 0 Dependencies  |
# +---------+         +-----------------+
Scenario: Wrong configuration key
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Blob Storage configuration items
    | Key                         | Account Name | Container     |
    | TestServices:C1D():WrongKey | blobaccount  | blobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D()' anticipating an exception
    Then an 'InvalidEnrollmentConfigurationException' is thrown
    And the tenant called 'Litware' should not have the id of the tenant called 'SvcC1D()' in its enrollments

# Dependency graph:
#
# +---------+         +-----------------+
# |         |         | Service with    |
# | Litware +---------| 1 Configuration |
# |         |         | 0 Dependencies  |
# +---------+         +-----------------+
Scenario: Wrong configuration type
    Given I have enrollment configuration called 'Config'
    And the enrollment configuration called 'Config' contains the following Table Storage configuration items
    | Key                         | Account Name | Table     |
    | TestServices:C1D():WrongKey | blobaccount  | blobcontainer |
    When I use the tenant store with the enrollment configuration called 'Config' to enroll the tenant called 'Litware' in the service called 'SvcC1D()' anticipating an exception
    Then an 'InvalidEnrollmentConfigurationException' is thrown
    And the tenant called 'Litware' should not have the id of the tenant called 'SvcC1D()' in its enrollments

# TODO: service requiring multiple config, and only some wrong
# TODO: missing config for dependencies