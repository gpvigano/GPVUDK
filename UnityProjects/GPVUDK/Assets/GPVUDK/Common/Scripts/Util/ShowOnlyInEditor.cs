using UnityEngine;

namespace GPVUDK
{
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
