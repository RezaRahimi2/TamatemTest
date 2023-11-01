using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GamePlayLogic;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GamePlayView
{
    public class GameLoader : MonoBehaviour
    {
        public string sceneAddress;
        public string prefabAddress;
        public LoadingView loadingView;

        private void Start()
        {
            loadingView.Initialize(OnClickLoadButton);
        }

        private async void OnClickLoadButton()
        {
            loadingView.OnStartLoading();
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive);
            handle.Completed += OnSceneLoadCompleted;
            while (!handle.IsDone)
            {
                float progress = handle.PercentComplete;
                Debug.unityLogger.Log(progress);
                await loadingView.SetLoadingProgress(progress);
                await UniTask.Yield();
            }
        }

        private async void OnSceneLoadCompleted(AsyncOperationHandle<SceneInstance> sceneOperationHandle)
        {
            GameObject chipView = await AddressableLoader.LoadAssetAsync<GameObject>(prefabAddress);
            
            if (chipView != null)
            {
                PrefabsManager.Instance.Initialize(chipView.GetComponent<ChipView>());
            }
            
            await GameController.Instance.Initialize();

            // Handle the completion of the scene load here
            loadingView.OnFinishedLoading(()=>
            {
                SceneManager.SetActiveScene(sceneOperationHandle.Result.Scene);
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
            });
        }
    }
}