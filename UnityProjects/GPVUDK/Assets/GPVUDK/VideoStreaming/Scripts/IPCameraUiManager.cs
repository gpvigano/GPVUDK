﻿using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// User interface manager for connecting to an IP camera.
    /// </summary>
    public class IPCameraUiManager : MonoBehaviour
    {
        [Tooltip("Input field for the IP camera URL (MJPEG video, not the HTTP page)")]
        [SerializeField]
        protected InputField urlInputField;
        [Tooltip("Input field for an optional login, if required to access the IP camera")]
        [SerializeField]
        protected InputField loginInputField;
        [Tooltip("Input field for an optional password, if required to access the IP camera")]
        [SerializeField]
        protected InputField passwordInputField;
        [Tooltip("Script that controls the IP camera rendering")]
        [SerializeField]
        protected IPCameraToMaterial iPCameraToMaterial;
        [Tooltip("Color for the URL input field when the connection fails.")]
        [SerializeField]
        protected Color connectionFailedColor = Color.red;
        [Tooltip("Color for the URL input field when the connection succeeds.")]
        [SerializeField]
        protected Color connectionSucceededColor = Color.green;
        [Tooltip("Default IP camera configuration")]
        [SerializeField]
        protected VideoStreamingSettings defaultSettings = new VideoStreamingSettings();
        [Tooltip("Start streaming on startup")]
        [SerializeField]
        private bool autoStartStreaming = false;

        /// <summary>
        /// IP camera video streming settings.
        /// </summary>
        public virtual VideoStreamingSettings VideoStreamSettings
        {
            get
            {
                return defaultSettings;
            }
        }

        /// <summary>
        /// Change the video streaming setting according to the UI content and restart video streaming.
        /// </summary>
        public virtual void ApplyUIChangesAndStartVideo()
        {
            UpdateVideoStreamSettings();
            RestartVideo();
        }

        /// <summary>
        /// Update the internal video streming setting according to the UI content
        /// </summary>
        public virtual void UpdateVideoStreamSettings()
        {
            if (urlInputField != null)
            {
                VideoStreamSettings.videoUrl = urlInputField.text;
            }
            if (loginInputField != null)
            {
                VideoStreamSettings.login = loginInputField.text;
            }
            if (passwordInputField != null)
            {
                VideoStreamSettings.password = passwordInputField.text;
            }
        }

        /// <summary>
        /// Restart video streaming using the current settings
        /// </summary>
        public virtual void RestartVideo()
        {
            StopVideo();
            SetUrl();
            StartVideo();
        }

        /// <summary>
        /// Stop the video streaming, if started
        /// </summary>
        public virtual void StopVideo()
        {
            iPCameraToMaterial.StopStream();
            if (urlInputField != null)
            {
                urlInputField.textComponent.color = Color.black;
            }
        }

        /// <summary>
        /// Start video streaming from the IP camera with last settings, restart if already running.
        /// </summary>
        public virtual void StartVideo()
        {
            iPCameraToMaterial.StartStream();
            if (urlInputField != null && iPCameraToMaterial.IsStreaming)
            {
                urlInputField.textComponent.color = connectionSucceededColor;
            }
        }

        /// <summary>
        /// Update the user interface according to the current Video Stream Settings.
        /// </summary>
        /// <param name="videoStreamSettings"></param>
        protected virtual void UpdateUI(VideoStreamingSettings videoStreamSettings)
        {
            if (urlInputField != null)
            {
                urlInputField.text = VideoStreamSettings.videoUrl;
            }
            if (loginInputField != null)
            {
                loginInputField.text = VideoStreamSettings.login;
            }
            if (passwordInputField != null)
            {
                passwordInputField.text = VideoStreamSettings.password;
            }
        }

        /// <summary>
        /// Called when the connection to the IP camera succeeds.
        /// </summary>
        protected virtual void OnConnectionSucceeded()
        {
            if (urlInputField != null)
            {
                urlInputField.textComponent.color = connectionSucceededColor;
            }
        }

        /// <summary>
        /// Called when the connection to the IP camera fails.
        /// </summary>
        protected virtual void OnConnectionFailed()
        {
            if (urlInputField != null)
            {
                urlInputField.textComponent.color = connectionFailedColor;
            }
        }

        /// <summary>
        /// Set URL and optionally login and password for the IP camera according to the current Video Stream Settings.
        /// </summary>
        protected virtual void SetUrl()
        {
            string url = VideoStreamSettings.videoUrl;
            string login = VideoStreamSettings.login;
            string password = VideoStreamSettings.password;
            iPCameraToMaterial.SetUrl(url, login, password);
        }


        /// <summary>
        /// Initialize the user interface and start video streaming if autoStartStreaming is true.
        /// </summary>
        protected virtual void Start()
        {
            UpdateUI(VideoStreamSettings);

            if(autoStartStreaming)
            {
                UpdateVideoStreamSettings();
                StartVideo();
            }
        }

        /// <summary>
        /// Search for scripts in the current game object if not yet set.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (iPCameraToMaterial == null)
            {
                iPCameraToMaterial = GetComponent<IPCameraToMaterial>();
            }
        }

        /// <summary>
        /// Initialize default settings
        /// </summary>
        protected virtual void Awake()
        {
            defaultSettings = VideoStreamSettings;
        }

        /// <summary>
        /// Register to IP camera events.
        /// </summary>
        protected virtual void OnEnable()
        {
            iPCameraToMaterial.ConnectionFailed += OnConnectionFailed;
            iPCameraToMaterial.ConnectionSucceeded += OnConnectionSucceeded;
        }

        /// <summary>
        /// Unregister from IP camera events.
        /// </summary>
        protected virtual void OnDisable()
        {
            iPCameraToMaterial.ConnectionFailed -= OnConnectionFailed;
            iPCameraToMaterial.ConnectionSucceeded -= OnConnectionSucceeded;
        }

        protected virtual void Update()
        {
            // TODO: maybe some code could be added here in future (e.g. timeout timer visualization)
        }
    }
}