using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableLoader
{
    public static async Task<T> LoadAssetAsync<T>(string address) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load asset at {address}: {handle.OperationException}");
            return null;
        }
    }
    
    public static async UniTask LoadSceneAsync(string address, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Failed to load scene at {address}: {handle.OperationException}");
        }
    }
}

