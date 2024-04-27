using UnityEngine;
using DoubleHeat.DependsOnAddressableAssets.SceneManagement;

namespace ProjectComet {

    public class Initiater : MonoBehaviour {

        [SerializeField] string _startingSceneName;

        [SerializeField] ScenesLoadingManager _sceneLoadingManager;

        
        void Start () {
            _sceneLoadingManager.LoadSceneAdditive(_startingSceneName);
        }

    }

}