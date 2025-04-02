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