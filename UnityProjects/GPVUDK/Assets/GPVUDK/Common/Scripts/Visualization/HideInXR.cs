using UnityEngine;
using UnityEngine.XR;

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
