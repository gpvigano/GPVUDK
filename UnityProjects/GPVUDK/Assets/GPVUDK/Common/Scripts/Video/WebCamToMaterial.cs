using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// Acquire images from a video stream coming from a local webcam
    /// </summary>
    public class WebCamToMaterial : MonoBehaviour
    {
        [Tooltip("Renderer with the material for the webcam texture")]
        [SerializeField]
        private Renderer webCamRenderer;
        [Tooltip("RawImage with the material for the webcam texture")]
        [SerializeField]
        private RawImage webCamRawImage;
        [Tooltip("Fit the RawImage dimensions to screen dimensions")]
        [SerializeField]
        private bool fitRawImageToScreen = true;
        [Tooltip("Width of the wecam texture")]
        [SerializeField]
        private int textureWidth = 2048;
        [Tooltip("Height of the wecam texture")]
        [SerializeField]
        private int textureHeight = 2048;

        private WebCamTexture webCamTexture = null;
        private Material webCamMaterial = null;
        private Material webCamImageMaterial = null;

        private void OnValidate()
        {
            if (webCamRenderer == null)
            {
                webCamRenderer = GetComponent<Renderer>();
            }
            if (webCamRawImage == null)
            {
                webCamRawImage = GetComponent<RawImage>();
            }
        }

        private void Awake()
        {
            if (webCamRawImage != null && fitRawImageToScreen)
            {
                webCamRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
                webCamRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            }

        }

        void Start()
        {
            if (WebCamTexture.devices.Length==0)
            {
                Debug.LogWarning(GetType().Name + " - no camera available.");
            }
            else
            {
                webCamTexture = new WebCamTexture(textureWidth, textureHeight);
                Debug.Log(GetType().Name + " - device: " + webCamTexture.deviceName);
                if (webCamRenderer != null)
                {
                    webCamMaterial = new Material(webCamRenderer.material);
                    webCamMaterial.mainTexture = webCamTexture;
                    webCamRenderer.material = webCamMaterial;
                }
                if (webCamRawImage != null)
                {
                    webCamImageMaterial = new Material(webCamRawImage.material);
                    webCamImageMaterial.mainTexture = webCamTexture;
                    webCamRawImage.material = webCamImageMaterial;
                }
                //if (webCamRenderer != null)
                //{
                //    webCamRenderer.material.mainTexture = webCamTexture;
                //}
                //if (webCamRawImage != null)
                //{
                //    webCamRawImage.material.mainTexture = webCamTexture;
                //}
                webCamTexture.Play();
            }
        }

        void Update()
        {
        }
    }
}
