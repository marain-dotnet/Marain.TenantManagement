#

Version 2 of `Marain.TenantManagement` is aligned with version 2 of `Corvus.Tenancy` and associated libraries. Version 3 `Marain.TenantManagement` moves to version 2 of `Corvus.Tenancy`, and this entails some significant changes, notably:

* Use of `Azure.Core` family Azure client libraries, with support for AAD authentication
* Application-defined container names
* Containers creation by applications during tenant onboarding (and [not automatically on demand as used to happen](https://github.com/corvus-dotnet/Corvus.Tenancy/blob/main/docs/adr/0003-no-automatic-storage-container-creation.md))

One upshot of these changes is that storage configuration stored in tenant properties is different—we needed to add new properties to support new client capabilities, but since the usage model wasn't backwards compatible, we also fixed some long-standing problems with the older format (e.g., some weird conventions in which properties didn't always do what their names suggested).

For example, storage configuration for `Marain.Operations` in the V2 model looks like this:


```json
"StorageConfiguration__operations": {
    "accountName": "maridgoperationsy3qxmrir",
    "container": "operations",
    "keyVaultName": "maridgoperationsy3qxmrir",
    "accountKeySecretName": "OperationsStorageAccountKey",
    "disableTenantIdPrefix": false
},
```

but in the V3 world, an exact equivalent might look like this:

```json
"StorageConfigurationV3__operations": {
    "accountName": "maridgoperationsy3qxmrir",
    "container": "513162c27e77e52411ececa40f1e615455b01fc5",
    "accessKeyInKeyVault": {
        "vaultName": "maridgoperationsy3qxmrir",
        "secretName": "OperationsStorageAccountKey",
    }
}
```

The key vault secret handling may look like some unnecessary arbitrary moving around of things, but it is part of adopting a uniform approach to Azure Key Vault and service identities across all sources, and it adds the ability to specify the identity with which to connect to key vault. (It defaults to the service identity, but you can configure alternatives.) These same changes also make it possible to authenticate to storage using a service identity instead of a key (by specifying `clientIdentity` instead of `accessKeyInKeyVault`).

The most substantial change, though, is that the `container` no longer contains a logical container name that the Corvus tenanted storage libraries rewrite as a tenanted name. Instead, that name choice happens during tenant onboarding, and the configuration in the tenant properties records the chosen container name. The v3 tenanted storage libraries require this container already to exist (whereas the v2 ones created it on first use automatically).

https://github.com/corvus-dotnet/Corvus.Tenancy/blob/main/docs/adr/0004-v2-to-v3-transition.md


What do we do if there are systems out there that require V2-style config? E.g., someone is using Marain.Operations but hasn't yet deployed an update to the version that is aware of V3-style config? One response to this might be: "Tough! If that's your situation, use V2 of `Marain.TenantManagement`. However, a problem with that is that it means that until every Marain service in the deployment has been upgraded to be V3-capable, nothing can start using the V3 approach. In cases where customers are using


Enrollment config files with only 