using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Deactivate this game object if not in Unity Editor
    /// </summary>
    public class ShowOnlyInEditor : MonoBehaviour
    {
#if !UNITY_EDITOR
        void Awake()
        {
            gameObject.SetActive(false);
        }
#endif
    }
}
