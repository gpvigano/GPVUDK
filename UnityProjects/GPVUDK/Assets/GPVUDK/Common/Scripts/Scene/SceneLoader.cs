using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace GPVUDK
{
    /// <summary>
    /// Manager for scene loading
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
#if UNITY_EDITOR 
        /// <summary>
        /// Drag here the scene to load.
        /// </summary>
        [Tooltip("Drag here the scene to load")]
        public SceneAsset sceneToLoad;
#endif
        /// <summary>
        /// Name of the scene to load (or drag the scene asset onto Scene To Load).
        /// </summary>
        [Tooltip("Name of the scene to load (or drag the scene asset onto Scene To Load)")]
        public string sceneName;
		
        /// <summary>
        /// Delay in seconds before starting loading (<0 = don't start).
        /// </summary>
        [Tooltip("Delay in seconds before starting loading (<0 = don't start)")]
        public float loadingDelay = 5.0f;
		
        /// <summary>
        /// Objects to be hidden while loading the scene.
        /// </summary>
        [Tooltip("Objects to be hidden while loading the scene")]
        public GameObject[] objectsToHideWhileLoading;

        protected bool loadingStarted = false;
        protected float elapsedTime = 0;
        private float startTime;

#if UNITY_EDITOR 
        private void OnValidate()
        {
            if (sceneToLoad != null)
            {
                sceneName = sceneToLoad.name;
            }
        }
#endif

        protected virtual void Start()
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            startTime = Time.realtimeSinceStartup;
        }

		
        protected virtual void Update()
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            if (loadingDelay>=0 && !loadingStarted && elapsedTime > loadingDelay)
            {
                LoadNextSceneAsync();
                loadingStarted = true;
                foreach(GameObject obj in objectsToHideWhileLoading)
                {
                    obj.SetActive(false);
                }
            }
        }
		

        protected void LoadNextSceneAsync()
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            }
            else
            {
                Debug.LogError(GetType().Name + " - Scene not set.");
            }
        }
    }
}
