Feature: Service manifest creation
	In order to describe the requirements of a service
	As a developer
	I want be able to create a service manifest

Scenario: Creating a manifest without providing a name
	When I create a service manifest without providing a service name
	Then an 'ArgumentNullException' is thrown
	
Scenario Outline: Creating a manifest with invalid service names
	When I create a service manifest with service name '<Service Name>'
	Then an 'ArgumentException' is thrown

	Examples:
	| Scenario Description | Service Name |
	| Empty string         | ""           |
	| Spaces only          | "  "         |
	| Tabs only            | "	"         |
