using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK.Video
{
    [RequireComponent(typeof(WebcamToMaterial))]
    public class WebcamUI : MonoBehaviour
    {
        [Tooltip("Text for displaying the list of devices")]
        [SerializeField]
        private Text webCamDeviceList;
        //[SerializeField]
        //private InputField webCamDeviceName;

        private WebcamToMaterial webCamToMaterial;

        /// <summary>
        /// Wrapper method forplay/pause from UI toggles.
        /// </summary>
        /// <param name="playing">If true play, else pause.</param>
        public void Play(bool playing)
        {
            if (playing)
            {
                webCamToMaterial.Play();
            }
            else
            {
                webCamToMaterial.Pause();
            }
        }

        /// <summary>
        /// Update the UI text with the devices list, the active one is highlighted.
        /// </summary>
        public void UpdateDeviceList()
        {
            if (webCamDeviceList != null)
            {
                StringBuilder deviceListText = new StringBuilder();
                WebCamDevice[] webCamDevices = WebCamTexture.devices;
                foreach (WebCamDevice device in webCamDevices)
                {
                    if(!string.IsNullOrEmpty(webCamToMaterial.DeviceName) && device.name==webCamToMaterial.DeviceName)
                    {
                        deviceListText.Append(">> ");
                    }
                    deviceListText.AppendLine(device.name);
                }
                webCamDeviceList.text = deviceListText.ToString();
            }
            //if (webCamDeviceName != null)
            //{
            //    webCamDeviceName.text = webCamToMaterial.DeviceName;
            //}
        }

        private void Awake()
        {
            webCamToMaterial = GetComponent<WebcamToMaterial>();
        }

        private void Start()
        {
            UpdateDeviceList();
        }

        private void OnEnable()
        {
            webCamToMaterial.InitializedEvent += OnWebCamInitialization;
        }

        private void OnDisable()
        {
            webCamToMaterial.InitializedEvent -= OnWebCamInitialization;
        }

        private void OnWebCamInitialization()
        {
            UpdateDeviceList();
        }
    }
}
