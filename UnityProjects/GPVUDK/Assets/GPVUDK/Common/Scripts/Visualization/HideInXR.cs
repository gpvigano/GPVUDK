using UnityEngine;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
using XRSettings = UnityEngine.VR.VRSettings;
#endif

namespace GPVUDK
{
    /// <summary>
    /// Deactivate the game object of this script if in XR (VR/AR) mode.
    /// </summary>
    public class HideInXR : MonoBehaviour
    {
        private void Awake()
        {
            if (XRSettings.enabled)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
