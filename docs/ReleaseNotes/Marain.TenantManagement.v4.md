# Release notes for Marain.TenantManagement v4.

## v4.0

Replaces `Newtonsoft.Json` with `System.Text.Json`.

### Breaking changes

**Serialization now supported only for `System.Text.Json`**

This is the main point of this upgrade, but it has consequences—anything relying on being able to use `Newtonsoft.Json` will not work. This entails `Corvus.Json.Serialization` instead of `Corvus.Extensions.Newtonsoft.Json`, which in turn means using `IJsonSerializerOptionsProvider` in place of `IJsonSerializerSettingsProvider`. It also means that the following dependencies have the following minimum versions:

| Dependency | Minimum version |
| --- | --- |
| `Menes.Abstractions` | 5.0.0 |
| `Corvus.Tenancy.Abstractions` | 4.0.0 |
| `Corvus.Storage.Azure.BlobStorage.Tenancy` | 4.0.0 |
| `Corvus.Storage.Azure.Cosmos.Tenancy` | 4.0.0 |
| `Corvus.Storage.Azure.TableStorage.Tenancy` | 4.0.0 |
| `Corvus.ContentHandling.Json` | 4.0.0 |

**Service Manifest types now require constructor initialization**

Some of the existing types were incompatible with `System.Text.Json` on account of defining read-only properties that return mutable collections. Whereas `Newtonsoft.Json` was happy to load data into collections returned by such properties, `System.Text.Json` currently doesn't. (This has tentatively been suggested as a feature for .NET 8.0.) While we could fix this by defining custom converters, it's better not to need that. Since we're making breaking changes in this version anyway in order to support `System.Text.Json`, it's better just to modify these types to work naturally.

We've taken the opportunity to turn the following into `record` types, with constructors requiring full initialization:

* `ServiceManifest`
* `ServiceDependency`
* `ServiceManifestRequiredConfigurationEntry` and derived types:
    * `ServiceManifestRequiredConfigurationEntryWithV2LegacySupport`
    * `ServiceManifestBlobStorageConfigurationEntry`
    * `ServiceManifestLegacyV2BlobStorageConfigurationEntry`
    * `ServiceManifestCosmosDbConfigurationEntry`
    * `ServiceManifestLegacyV2CosmosDbConfigurationEntry`
    * `ServiceManifestLegacyV2TableStorageConfigurationEntry`
    * `ServiceManifestLegacyV2TableStorageConfigurationEntry`

For `ServiceManifest` and `ServiceDependency`, instance methods have moved out into extension methods. For the various `ServiceManifestRequiredConfigurationEntry`-derived types, the instance methods are all virtual, which helps drive the configuration-type-specific behaviour, so those have remained.