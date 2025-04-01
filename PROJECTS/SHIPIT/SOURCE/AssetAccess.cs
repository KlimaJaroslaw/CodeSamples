using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetAccess : MonoBehaviour
{
    private Dictionary<AssetIdentifier, AsyncOperationHandle> _handles = new Dictionary<AssetIdentifier, AsyncOperationHandle>();
    //private bool _disposed;
    //private static Dictionary<int, string> assetKeyDictionary = new Dictionary<int, string>();
    private static Dictionary<int, LanguageInfo> textKeyDictionary = new Dictionary<int, LanguageInfo>();

    #region Unity Methods
    void Start()
    {
        _handles = new Dictionary<AssetIdentifier, AsyncOperationHandle>();        
    }
    #endregion

    #region Asset Access
    public async Task<T> LoadAsset<T>(string path) where T : class
    {
        var handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;
        this._handles[new AssetIdentifier()] = handle;            
        return handle.Result;
    }

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

    public LanguageInfo LoadTextInfo(AssetIdentifier id)
    {        
        if(id==null)
            return null;

        if(textKeyDictionary.TryGetValue(id.assetId, out LanguageInfo languageInfo))
        {
            return languageInfo;    
        }        

        LogBook.LogError(new NullReferenceException($"Text Asset Not Found. {id?.assetId}"),this);
        return null;
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