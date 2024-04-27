using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace DoubleHeat.DependsOnAddressableAssets.SceneManagement {
    
    public class ScenesLoadingManager : MonoBehaviour {

        public event EventHandler<EventArgs> SceneSwitchingOfSlotCompleted;
        public event EventHandler<EventArgs> SceneUnloaded;
        public event EventHandler<EventArgs> SceneLoaded;


        [Header("Scenes")]
        [SerializeField] SceneInfo[] _sceneInfos;



        Dictionary<string, SceneLoadingHandle> _sceneLoadingHandleSlots = new Dictionary<string, SceneLoadingHandle>();
        List<SceneLoadingHandle> _currentSceneLoadingHandles = new List<SceneLoadingHandle>();



        // For testing
        #if UNITY_EDITOR
            [ContextMenu("First Slot Switch To Next Scene")]
            public bool FirstSlotSwitchToNextScene () {
                if (_sceneLoadingHandleSlots.Count > 0) {
                    var firstSlot = _sceneLoadingHandleSlots.ElementAt(0);
                    if (firstSlot.Value != null) {

                        int currentIndexOfScenes = Array.FindIndex(_sceneInfos, info => info.Name == firstSlot.Value.SceneName);
                        if (currentIndexOfScenes != -1) {
                            int nextIndexScenes = (currentIndexOfScenes + 1) % _sceneInfos.Length;
                            
                            SwtichSceneOfSlotTo(firstSlot.Key, _sceneInfos[nextIndexScenes].Name);
                            return true;
                        }
                    }
                }
                Debug.Log("Operation failed.");
                return false;
            }
        #endif


        public void SwtichSceneOfSlotTo (string slotName, string newSceneName, Action initSceneAction = null) {

            Action loadingSceneIntoSlot = () => {
                SceneLoadingHandle handle = LoadSceneAdditive(GetSceneInfoByName(newSceneName), initSceneAction);
                handle.OperationHandle.Completed += loadingHandle => {
                    SceneSwitchingOfSlotCompleted?.Invoke(this, new SlotEventArgs{ sceneName = newSceneName, slotName = slotName });
                };
                
                _sceneLoadingHandleSlots[slotName] = handle;
            };

            if (_sceneLoadingHandleSlots.ContainsKey(slotName)) {
                SceneLoadingHandle handle = _sceneLoadingHandleSlots[slotName];

                if (handle != null) {
                    // Is Not Busy
                    _sceneLoadingHandleSlots[slotName] = null;

                    UnloadScene(handle, loadingSceneIntoSlot);
                }
            }
            else {
                _sceneLoadingHandleSlots.Add(slotName, null);
                loadingSceneIntoSlot?.Invoke();
            }

        }



        public void PutLoadedSceneIntoNewSlot (string sceneName, string slotName) {
            SceneLoadingHandle targetHandle = _currentSceneLoadingHandles.Find(handle => handle.SceneName == sceneName);
            if (targetHandle != null) {
                _sceneLoadingHandleSlots.Add(slotName, targetHandle);
            }
            else {
                Debug.LogWarning("Target scene is note found.");
            }
        }

        public void LoadSceneAdditive (string sceneName, Action initSceneAction = null) {
            LoadSceneAdditive(GetSceneInfoByName(sceneName), initSceneAction);
        }

        protected SceneLoadingHandle LoadSceneAdditive (SceneInfo sceneInfo, Action initSceneAction = null) {
            if (sceneInfo != null) {
                var loadingHandle = LoadSceneFromAssetRef(sceneInfo.SceneAssetRef);
                loadingHandle.Completed += handle => initSceneAction?.Invoke();

                SceneLoadingHandle handle = new SceneLoadingHandle(sceneInfo.Name, loadingHandle);
                _currentSceneLoadingHandles.Add(handle);
                return handle;
            }
            Debug.LogWarning("Target scene not found when trying to load scene!");
            return null;
        }

        public void UnloadScene (string sceneName, Action endCallback = null) {
            UnloadScene(_currentSceneLoadingHandles.Find(handle => handle.SceneName == sceneName));
        }

        protected void UnloadScene (SceneLoadingHandle targetSceneLoadingHandle, Action endCallback = null) {
            _currentSceneLoadingHandles.Remove(targetSceneLoadingHandle);
            if (targetSceneLoadingHandle != null) {
                UnloadScene(targetSceneLoadingHandle.OperationHandle, endCallback);
            }
        }



        AsyncOperationHandle<SceneInstance> LoadSceneFromAssetRef (AssetReference sceneAssetRef) {
            if (sceneAssetRef != null) {
                var loadingSceneHandle = sceneAssetRef.LoadSceneAsync(LoadSceneMode.Additive);
                loadingSceneHandle.Completed += OnSceneLoadingCompleted;

                return loadingSceneHandle;
            }
            else {
                Debug.LogError("Trying to load scene from empty asset reference!");
            }
            return default;
        }


        AsyncOperationHandle<SceneInstance> UnloadScene (AsyncOperationHandle<SceneInstance> sceneLoadingOperationHandle, Action endCallback = null) {

            if (sceneLoadingOperationHandle.IsValid()) {
                var unloadingHandle = Addressables.UnloadSceneAsync(sceneLoadingOperationHandle);
                unloadingHandle.Completed += OnSceneUnloadingCompleted;
                unloadingHandle.Completed += handle => endCallback?.Invoke();

                return unloadingHandle;
            }
            return default;
        }


        protected SceneInfo GetSceneInfoByName (string sceneName) {
            return Array.Find(_sceneInfos, info => info.Name == sceneName);
        }

        protected bool TryGetSceneInfoByName (string sceneName, out SceneInfo sceneInfo) {
            sceneInfo = GetSceneInfoByName(sceneName);
            return sceneInfo != null;
        }


        protected virtual void OnSceneUnloadingCompleted (AsyncOperationHandle<SceneInstance> handle) {
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                SceneUnloaded?.Invoke(this, EventArgs.Empty);
            }
            else {
                Debug.LogError("Unload scene failed!");
            }
        }

        protected virtual void OnSceneLoadingCompleted (AsyncOperationHandle<SceneInstance> handle) {
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                SceneLoaded?.Invoke(this, EventArgs.Empty);
            }
            else {
                Debug.LogError("Load scene failed!");
            }
        }


        [Serializable]
        public class SceneInfo {
            [SerializeField] string _name;
            public string Name => _name;

            [SerializeField] AssetReference _sceneAssetRef;
            public AssetReference SceneAssetRef => _sceneAssetRef;
        }

        public class SceneEventArgs : EventArgs {
            public string sceneName;
        }

        public class SlotEventArgs : SceneEventArgs {
            public string slotName;
        }


        protected class SceneLoadingHandle {
            public string SceneName {get; private set;}
            public AsyncOperationHandle<SceneInstance> OperationHandle {get; private set;}

            public SceneLoadingHandle (string sceneName, AsyncOperationHandle<SceneInstance> handle) {
                SceneName = sceneName;
                OperationHandle = handle;
            }
        }

    }

}