#if UNITY_WSA_10_0 && !UNITY_EDITOR
#define HOLOLENS_AVAILABLE
#endif

using UnityEngine;
#if HOLOLENS_AVAILABLE
using System.Linq;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.WebCam;
#else
using UnityEngine.VR.WSA.WebCam;
#endif
#endif

namespace GPVUDK.HoloLens
{
    /// <summary>
    /// Capture videos of mixed reality sessions from the HoloLens
    /// </summary>
    public class MixedRealityCapture : MonoBehaviour
    {
        [Tooltip("Start automatically on start")]
        [SerializeField]
        private bool autoStart = false;
        [Tooltip("Max length of video clips in seconds")]
        [SerializeField]
        private float maxRecordingTime = 300.0f;
        [Tooltip("Video file name (time code will be appended)")]
        [SerializeField]
        private string fileNameTemplate = "Video";

#if HOLOLENS_AVAILABLE
        private bool startSoon = false;
        private bool stopSoon = false;
        private VideoCapture videoCapture = null;
        private float stopRecordingTimer = float.MaxValue;
        private CameraParameters cameraParameters;
        private bool isInitialized = false;
#endif

        public bool IsCapturing
        {
            get
            {
#if HOLOLENS_AVAILABLE
                return (videoCapture != null && videoCapture.IsRecording);
#else
                return false;
#endif
            }
        }

        public void ToggleMixedRealityCapture(bool on)
        {
            if (on)
            {
                StartMixedRealityCapture();
            }
            else
            {
                StopMixedRealityCapture();
            }
        }

        public void StopMixedRealityCapture()
        {
#if HOLOLENS_AVAILABLE
            if (IsCapturing)
            {
                videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            }
#endif
        }

        public void InitMixedRealityCapture()
        {
#if HOLOLENS_AVAILABLE
            if (isInitialized)
            {
                return;
            }
            isInitialized = true;

            Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Debug.Log(cameraResolution);

            float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
            Debug.Log(cameraFramerate);

            cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 1.0f;
            cameraParameters.frameRate = cameraFramerate;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
            VideoCapture.CreateAsync(true, OnVideoCaptureResourceCreated);
#endif
        }

        public void StartMixedRealityCapture()
        {
#if HOLOLENS_AVAILABLE
            if (IsCapturing)
            {
                return;
            }

            if (videoCapture == null)
            {
                startSoon = true;
                if (!isInitialized)
                {
                    InitMixedRealityCapture();
                }
            }
            else
            {
                StartVideoCapture();
            }
#endif
        }

#if HOLOLENS_AVAILABLE

        private void StartVideoCapture()
        {

            string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
            string filename = string.Format("{0}_{1}.mp4", fileNameTemplate, timeStamp);
            string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            filepath = filepath.Replace("/", @"\");
            videoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
        }

        private void OnVideoCaptureResourceCreated(VideoCapture videoCapture)
        {

            if (videoCapture != null)
            {
                this.videoCapture = videoCapture;
                Debug.Log("Created VideoCapture Instance!");
                this.videoCapture.StartVideoModeAsync(cameraParameters,
                                                        VideoCapture.AudioState.ApplicationAndMicAudio,
                                                        this.OnStartedVideoCaptureMode);
            }
            else
            {
                Debug.LogError("Failed to create VideoCapture Instance!");
                isInitialized = false;
            }
        }

        private void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("Started Video Capture Mode");
                if (startSoon)
                {
                    StartVideoCapture();
                }
            }
            else
            {
                Debug.LogError("Failed to start Video Capture Mode");
            }
        }

        private void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("Stopped Video Capture Mode");
            }
            else
            {
                Debug.LogError("Failed to stop Video Capture Mode");
            }
        }

        private void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("Stopped Recording Video!");
                if (stopSoon)
                {
                    videoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
                }
            }
            else
            {
                Debug.LogError("Failed to stop Recording Video");
            }
        }

        private void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("Started Recording Video");
                stopRecordingTimer = Time.time + maxRecordingTime;
            }
            else
            {
                Debug.LogError("Failed to start Recording Video");
            }
        }

        private void Start()
        {
            if (autoStart)
            {
                StartMixedRealityCapture();
            }
        }

        private void OnApplicationQuit()
        {
            stopSoon = true;
            StopMixedRealityCapture();
            CancelInvoke("StartMixedRealityCapture");
        }

        private void Update()
        {
            if (!IsCapturing)
            {
                return;
            }

            if (Time.time > stopRecordingTimer)
            {
                StopMixedRealityCapture();
                Invoke("StartMixedRealityCapture", 1f);
            }
        }
#else
        private void Start()
        {
            Debug.LogWarning(GetType().Name + " works only in a Mixed Reality Environment.\nParameters:"
                + "autoStart = " + autoStart
                + ", maxRecordingTime = " + maxRecordingTime
                + ", fileNameTemplate = " + fileNameTemplate
                );
        }
#endif
    }
}