[:arrow_up: ShipIt (Project)](/PROJECTS/SHIPIT/SHIPIT.md)

[:arrow_right: NEXT MODULE: Abilities & Linear Modifiers](/PROJECTS/SHIPIT/MODULES/ABILITIES.md)

# Asset Access
Centralized mechanism for asset load using Unity Addressables.

## SOURCE CODE FILES
:link: [AssetIdentifier.cs](/PROJECTS/SHIPIT/SOURCE/AssetIdentifier.cs)\
:link: [AssetAccess.cs](/PROJECTS/SHIPIT/SOURCE/AssetAccess.cs)\

# Asset idnetification
To identify each asset (example: image, *Unity's Scriptable Object*, etc) I use Scriptable Object Asset Identifer:
``` csharp
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AssetIdentifier", menuName = "AssetIdentification/AssetIdentifier")]
public class AssetIdentifier : ScriptableObject
{
    public int assetId;
    public string assetName;
}
```
###### Code @ AssetIdentifer.cs (fragment)
Field assetName corresponds to value of Unity *Addresables asset* key

This approach enables scalability and makes asset management easier. It also simplify serialization process.

In scenario where player may choose one of multiple skin of character, I could create as many AssetIdentifer objects as needed and just load one at the time.


# Asset loading
AssetAccess class is responsibe for loading any assets, it derives from Unity's MonoBehaviour class and make use of Start and OnDestory methods.

``` csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetAccess : MonoBehaviour
{
    private Dictionary<AssetIdentifier, AsyncOperationHandle> _handles = new Dictionary<AssetIdentifier, AsyncOperationHandle>();

    #region Unity Methods
    void Start()
    {
        _handles = new Dictionary<AssetIdentifier, AsyncOperationHandle>();        
    }
    #endregion

    #region Disposing
    void OnDestroy()
    {
        this.ReleaseHandles();    
    }        

    private void ReleaseHandles()
    {
        foreach (var handle in _handles.Values)
        {
            Addressables.Release(handle);
        }
        _handles.Clear();
    }    
    #endregion
}
```
###### Code @ AssetAccess.cs (fragment)

To keep track of already loaded assets handles, AssetAccess has private field of type Dictionary<AssetIdentifier, AsyncOperationHandle>.

To load any asset one must call LoadAsset<T>(AssetIdentifier id) where T : class

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetAccess : MonoBehaviour
{
    private Dictionary<AssetIdentifier, AsyncOperationHandle> _handles = new Dictionary<AssetIdentifier, AsyncOperationHandle>();

    #region Asset Access
    public async Task<T> LoadAsset<T>(AssetIdentifier id) where T : class
    {
        if (id==null)
            return null;

        if (_handles.TryGetValue(id, out AsyncOperationHandle existingHandle))
        {
            return existingHandle.Result as T;
        }

        AsyncOperationHandle handle = Addressables.LoadAssetAsync<T>(id.assetName);
        await handle.Task;

        _handles[id] = handle;
        return handle.Result as T;        
    }
    #endregion
}
```
###### Code @ AssetAccess.cs (fragment)

AssetAccess firstly checks if queried asset is already loaded, and if not it calls Addressables API to return asset and save it in its dictionary.