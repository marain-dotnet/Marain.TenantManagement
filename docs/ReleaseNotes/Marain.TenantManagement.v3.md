# Release notes for Marain.TenantManagement v3.

## v3.0

Targets .NET 6.0 only.

Uses Corvus.Tenancy v3, and the corresponding Corvus.Storage libraries.

Separate components for each storage technology, so that a dependency on `Marain.TenantManagement` doesn't bring in dependencies on all storage techs (with corresponding version-specific dependencies.)


### Breaking changes

This is very different from v2, and upgrading will require rework, because every one of the new features described above is a breaking change.