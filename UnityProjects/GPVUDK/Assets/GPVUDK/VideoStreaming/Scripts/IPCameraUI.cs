using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK.Video
{
    /// <summary>
    /// User interface manager for connecting to an IP camera.
    /// </summary>
    public class IPCameraUI : MonoBehaviour
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
        [Tooltip("Button for starting the video streaming")]
        [SerializeField]
        protected Button startButton;
        [Tooltip("Button for stopping the video streaming")]
        [SerializeField]
        protected Button stopButton;
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

        protected Color connectionDefaultColor = Color.black;

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
            iPCameraToMaterial.Stop();
        }

        /// <summary>
        /// Start video streaming from the IP camera with last settings, restart if already running.
        /// </summary>
        public virtual void StartVideo()
        {
            iPCameraToMaterial.Play();
        }

        /// <summary>
        /// Update the user interface according to the current Video Stream Settings.
        /// </summary>
        /// <param name="videoStreamSettings"></param>
        protected virtual void UpdateUI(VideoStreamingSettings videoStreamSettings)
        {
            if (urlInputField != null)
            {
                urlInputField.text = videoStreamSettings.videoUrl;
            }
            if (loginInputField != null)
            {
                loginInputField.text = videoStreamSettings.login;
            }
            if (passwordInputField != null)
            {
                passwordInputField.text = videoStreamSettings.password;
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
            if (startButton != null)
            {
                startButton.interactable = false;
            }
            if (stopButton != null)
            {
                stopButton.interactable = true;
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
            if (startButton != null)
            {
                startButton.interactable = true;
            }
            if (stopButton != null)
            {
                stopButton.interactable = false;
            }
        }


        private void IPCameraToMaterial_ConnectionClosed()
        {
            if (urlInputField != null)
            {
                urlInputField.textComponent.color = connectionDefaultColor;
            }
            if (startButton != null)
            {
                startButton.interactable = true;
            }
            if (stopButton != null)
            {
                stopButton.interactable = false;
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
            if (string.IsNullOrEmpty(url))
            {
                url = iPCameraToMaterial.Url;
                login = iPCameraToMaterial.Login;
                password = iPCameraToMaterial.Password;
            }
            else
            {
                iPCameraToMaterial.SetUrl(url, login, password);
            }
        }

        /// <summary>
        /// Initialize default settings
        /// </summary>
        protected virtual void Awake()
        {
            if (urlInputField != null)
            {
                connectionDefaultColor = urlInputField.textComponent.color;
            }
            if (startButton != null)
            {
                startButton.interactable = true;
            }
            if (stopButton != null)
            {
                stopButton.interactable = false;
            }
            UpdateUI(defaultSettings);
        }

        /// <summary>
        /// Initialize the user interface and start video streaming if autoStartStreaming is true.
        /// </summary>
        protected virtual void Start()
        {
            if (autoStartStreaming)
            {
                UpdateVideoStreamSettings();
                SetUrl();
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
        /// Register to IP camera events.
        /// </summary>
        protected virtual void OnEnable()
        {
            iPCameraToMaterial.ConnectionFailed += OnConnectionFailed;
            iPCameraToMaterial.ConnectionSucceeded += OnConnectionSucceeded;
            iPCameraToMaterial.ConnectionClosed += IPCameraToMaterial_ConnectionClosed;
        }

        /// <summary>
        /// Unregister from IP camera events.
        /// </summary>
        protected virtual void OnDisable()
        {
            iPCameraToMaterial.ConnectionFailed -= OnConnectionFailed;
            iPCameraToMaterial.ConnectionSucceeded -= OnConnectionSucceeded;
            iPCameraToMaterial.ConnectionClosed -= IPCameraToMaterial_ConnectionClosed;
        }

        protected virtual void Update()
        {
            // TODO: maybe some code could be added here in future (e.g. timeout timer visualization)
        }
    }
}
