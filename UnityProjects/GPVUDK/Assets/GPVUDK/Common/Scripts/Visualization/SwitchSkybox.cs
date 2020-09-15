using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Switch the skybox of a camera on or off
    /// </summary>
    public class SwitchSkybox : MonoBehaviour
    {
        /// <summary>
        /// Camera to be modified (main camera if empty).
        /// </summary>
        [Tooltip("Camera to be modified (main camera if empty)")]
        public Camera controlledCamera = null;
		
        /// <summary>
        /// Switch the skybox on or off on startup, according to Initially On value.
        /// </summary>
        [Tooltip("Switch the skybox on or off on startup, according to Initially On value")]
        public bool switchOnStart = false;
		
        /// <summary>
        /// If Switch On Start is checked the skybox is switched on or off on startup, according to this value.
        /// </summary>
        [Tooltip("If Switch On Start is checked the skybox is switched on or off on startup, according to this value")]
        public bool initiallyOn = false;

		
        /// <summary>
        /// Switch the skybox of the selected camera (or the main camera) on or off
        /// </summary>
        /// <param name="on"></param>
        public void Switch(bool on)
        {
            if (controlledCamera != null)
            {
                controlledCamera.clearFlags = on ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
            }

        }

		
        private void Awake()
        {
            if (controlledCamera == null)
            {
                controlledCamera = Camera.main;
            }
        }

		
        private void Start()
        {
            if (switchOnStart)
            {
                Switch(initiallyOn);
            }
        }
    }
}
