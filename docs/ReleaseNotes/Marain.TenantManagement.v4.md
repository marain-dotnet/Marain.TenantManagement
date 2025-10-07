# Release notes for Marain.TenantManagement v4.0

## v4.0

Targets .NET 8.0 with major modernization of CLI, JSON serialization, and testing frameworks.

### Changes by Project

#### All Projects
- **Upgraded to .NET 8.0**: All projects now target .NET 8.0 (upgraded from .NET 6.0)
- **Dependency Updates**: Updated to Corvus packages v4.0.1 and Microsoft.Extensions.* v8.0.*
- **JSON Serialization Migration**: Replaced Newtonsoft.Json with System.Text.Json throughout the solution

#### Marain.TenantManagement.Abstractions
- **Dependency Updates**: Updated [`Corvus.Tenancy.Abstractions`](Solutions/Marain.TenantManagement.Abstractions/Marain.TenantManagement.Abstractions.csproj:18) to v4.0.1
- **JSON Migration**: Replaced `IJsonSerializerSettingsProvider` with `IJsonSerializerOptionsProvider` in [`TenantManagementServiceCollectionExtensions`](Solutions/Marain.TenantManagement.Abstractions/Microsoft/Extensions/DependencyInjection/TenantManagementServiceCollectionExtensions.cs:1)
- **Exception Cleanup**: Removed `System.Runtime.Serialization` dependencies from exception classes

#### Marain.Services.Tenancy & Marain.Services.Tenancy.Testing
- **JSON Migration**: Updated [`InMemoryTenantProvider`](Solutions/Marain.Services.Tenancy.Testing/Marain/TenantManagement/Testing/InMemoryTenantProvider.cs:1) and [`TransientTenantManager`](Solutions/Marain.Services.Tenancy.Testing/Marain/TenantManagement/Testing/TransientTenantManager.cs:1) to use System.Text.Json
- **Simplified Serialization**: Streamlined JSON handling methods and removed Newtonsoft.Json dependencies

#### Marain.TenantManagement.Azure.* (BlobStorage, Cosmos, TableStorage)
- **Dependency Updates**: Updated Corvus.Storage packages to v4.0.1
- **JSON Migration**: Updated service manifest entries to work with System.Text.Json serialization

#### Marain.TenantManagement.Cli
- **Complete CLI Framework Migration**: Migrated from System.CommandLine to Spectre.Console.Cli ([commit a114e3b](https://github.com/marain-dotnet/Marain.TenantManagement/commit/a114e3b8f4474109b9911fdb368e6e4a17bd551f))
- **Package Dependencies**:
  - **Removed**: `System.CommandLine`, `ConsoleTables`, `Corvus.Identity.MicrosoftRest`
  - **Added**: [`Spectre.Console (0.50.0)`](Solutions/Marain.TenantManagement.Cli/Marain.TenantManagement.Cli.csproj:53), [`Spectre.Console.Cli (0.50.0)`](Solutions/Marain.TenantManagement.Cli/Marain.TenantManagement.Cli.csproj:54)
- **Enhanced Visual Experience**:
  - Color-coded output (green for success, red for errors, blue for info, yellow for warnings)
  - Rich progress indicators with ✓ and ✗ symbols
  - Enhanced tables with rounded borders and column formatting
  - Tree hierarchy visualization for tenant structures
  - Structured error panels with proper formatting
- **New Infrastructure Components**:
  - [`TypeRegistrar`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Infrastructure/TypeRegistrar.cs:1) - Integrates Spectre.Console.Cli with Microsoft.Extensions.DependencyInjection
  - [`TypeResolver`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Infrastructure/TypeResolver.cs:1) - Handles dependency resolution for commands
  - [`CommandConfigurationExtensions`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Infrastructure/CommandConfigurationExtensions.cs:1) - Centralizes command registration and configuration
- **Updated Commands** (All 7 commands redesigned with enhanced UX):
  - [`InitialiseCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/InitialiseCommand.cs:1)
  - [`CreateClientTenantCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/CreateClientTenantCommand.cs:1)
  - [`CreateServiceTenantCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/CreateServiceTenantCommand.cs:1)
  - [`EnrollCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/EnrollCommand.cs:1)
  - [`UnenrollCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/UnenrollCommand.cs:1)
  - [`ShowHierarchyCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/ShowHierarchyCommand.cs:1)
  - [`ListRequiredConfigurationForServiceCommand`](Solutions/Marain.TenantManagement.Cli/Marain/TenantManagement/Cli/Commands/ListRequiredConfigurationForServiceCommand.cs:1)
- **Migration Summary**: Added comprehensive [`MIGRATION_SUMMARY.md`](Solutions/Marain.TenantManagement.Cli/MIGRATION_SUMMARY.md:1) documenting the CLI modernization process
- **JSON Migration**: Updated CLI commands to use System.Text.Json for manifest processing

#### Marain.TenantManagement.Specs
- **Testing Framework Migration**: Migrated from SpecFlow to ReqnRoll framework ([commit f8cd4fe](https://github.com/marain-dotnet/Marain.TenantManagement/commit/f8cd4fea47589f8c30692e34bd574ee5d75651dc))
- **Configuration Updates**:
  - Replaced [`specflow.json`](Solutions/Marain.TenantManagement.Specs/specflow.json) with [`reqnroll.json`](Solutions/Marain.TenantManagement.Specs/reqnroll.json:1)
  - Updated package references from `Corvus.Testing.SpecFlow` to `Corvus.Testing.ReqnRoll`
- **Step Definition Updates**: Updated all step definition classes to use ReqnRoll framework
- **JSON Migration**: Updated [`ManifestSteps`](Solutions/Marain.TenantManagement.Specs/Steps/ManifestSteps.cs:1) and related files for System.Text.Json compatibility

### Infrastructure & Build System Changes
- **GitHub Actions Migration**: Migrated from Azure DevOps pipelines to GitHub Actions workflow ([`.github/workflows/build.yml`](.github/workflows/build.yml:1))
- **ZeroFailed Integration**: Added ZeroFailed configuration ([`.zf/config.ps1`](.zf/config.ps1:1))
- **Dependency Analysis**: Removed StyleCop.Analyzers and Roslynator.Analyzers dependencies
- **Build Script Updates**: Enhanced [`build.ps1`](build.ps1:1) for improved build process

### Major Features

#### CLI Modernization
- **Professional User Experience**: Complete visual overhaul with modern console interface
- **Enhanced Error Handling**: Structured error display with user-friendly messages
- **Rich Data Visualization**: Tables, trees, and panels for better data presentation
- **Improved Command Structure**: Better help system with examples and clearer documentation

#### JSON Serialization Upgrade
- **System.Text.Json Migration**: Moved from Newtonsoft.Json to Microsoft's built-in JSON serializer
- **Performance Improvements**: Faster serialization/deserialization with lower memory usage
- **Better .NET Integration**: Improved compatibility with modern .NET features

#### Testing Framework Modernization
- **ReqnRoll Migration**: Upgraded from SpecFlow to the actively maintained ReqnRoll framework
- **Improved Test Reliability**: Better test framework stability and maintenance
- **Enhanced BDD Support**: Continued support for behavior-driven development with modern tooling

### Breaking Changes
- **Target Framework**: All projects now require .NET 8.0
- **CLI Framework**: Custom CLI integrations using System.CommandLine will need updates for Spectre.Console.Cli
- **JSON Serialization**: Applications using Newtonsoft.Json-specific features may require updates
- **Testing Framework**: Custom SpecFlow extensions will need migration to ReqnRoll

### Migration Notes
This version focuses on modernizing the development and user experience while maintaining all existing tenant management functionality. The CLI provides significantly improved usability, and the JSON migration offers better performance and .NET integration.

Existing tenant management operations, service manifests, and enrollment configurations remain fully compatible. The V2 to V3 migration support introduced in previous versions continues to work as expected.