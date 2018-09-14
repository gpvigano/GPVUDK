// code inspired by:
// https://answers.unity.com/questions/1151512/show-video-from-ip-camera-source.html

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;
#if UNITY_WSA_10_0 && WINDOWS_UWP
using System.Net.Http;
#else
using System.Net;
#endif
using System.Net.Sockets;
using System.IO;

namespace GPVUDK
{
    /// <summary>
    /// Acquire images from a video stream coming from a remote IP camera
    /// </summary>
    public class IPCameraToMaterial : MonoBehaviour
    {
        [Tooltip("Renderer with the material texture for the IP camera")]
        [SerializeField]
        private Renderer frameRenderer;
        [Tooltip("Image with the material texture for the IP camera")]
        [SerializeField]
        private Image frameImage;
        [Tooltip("URL of the IP camera")]
        [SerializeField]
        private string url = "http://10.2.13.100:8080/video";
        [Tooltip("Optional login required to access the IP camera")]
        [SerializeField]
        private string login = null;
        [Tooltip("Optional password required to access the IP camera")]
        [SerializeField]
        private string password = null;
        [Tooltip("Start streaming on startup")]
        [SerializeField]
        private bool autoStartStreaming = false;
        [Tooltip("Restart on connection lost")]
        [SerializeField]
        private bool restartOnError = true;
        [Tooltip("Timeout for video stream reading operations")]
        [SerializeField]
        private float readTimeout = 0.2f;
        [Tooltip("Time before giving up after read timeouts")]
        [SerializeField]
        private float giveUpTimeout = 10f;
        [Tooltip("Time before retrying if the connection is lost")]
        [SerializeField]
        private float reconnectTimeout = 1f;

        private Byte[] jpegData;

        private Texture2D texture;
        private int savedWidth = 0;
        private int savedHeight = 0;
        private int savedTexWidth = 0;
        private int savedTexHeight = 0;
        private string thisClassName;
        private bool firstFrame = false;
        private bool restartStream = false;
        private bool resizeFrameImage = false;

        /// <summary>
        /// Flag set to true when the video streming is running.
        /// </summary>
        public bool IsStreaming { get; private set; }

        /// <summary>
        /// Event triggered when on connection error.
        /// </summary>
        public event Action ConnectionFailed;
        /// <summary>
        /// Event triggered when the connection succeeds.
        /// </summary>
        public event Action ConnectionSucceeded;

        /// <summary>
        /// Set URL and optionally login and password for the IP camera.
        /// </summary>
        /// <param name="url">URL of the IP camera</param>
        /// <param name="login">Optional login required to access the IP camera</param>
        /// <param name="password">Optional password required to access the IP camera</param>
        public void SetUrl(string url, string login = null, string password = null)
        {
            bool wasStreaming = IsStreaming;
            StopStream();
            this.url = url;
            this.login = login;
            this.password = password;
            if (wasStreaming)
            {
                StartStream();
            }
        }

        /// <summary>
        /// Stop video streaming, if running.
        /// </summary>
        public void StopStream()
        {
            if (IsStreaming)
            {
                Debug.Log(thisClassName + ": stream stopped");
            }
            IsStreaming = false;
            StopCoroutine(GetVideoStream());
            if (frameImage != null)
            {
                frameImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Start video streaming, restart if already running.
        /// </summary>
        public void StartStream()
        {
            StartCoroutine(GetVideoStream());
        }

        protected virtual void OnConnectionFailed()
        {
            if (ConnectionFailed != null)
            {
                ConnectionFailed();
            }
        }

        protected virtual void OnConnectionSucceeded()
        {
            if (ConnectionSucceeded != null)
            {
                ConnectionSucceeded();
            }
        }

        protected virtual IEnumerator OnStreamLost()
        {
            OnConnectionFailed();
            if (restartOnError)
            {
                yield return new WaitForSecondsRealtime(reconnectTimeout);
                restartStream = true;
            }
        }

        private IEnumerator GetFrames(Stream stream)
        {
#if UNITY_WSA_10_0 && WINDOWS_UWP
#else
            stream.ReadTimeout = (int)(readTimeout * 1000f);
#endif
            if (frameRenderer != null)
            {
                frameRenderer.material.color = Color.white;
                frameRenderer.material.mainTexture = texture;
            }
            if (frameImage != null)
            {
                frameImage.material.color = Color.white;
                frameImage.material.mainTexture = texture;
                frameImage.gameObject.SetActive(true);
            }
            IsStreaming = true;
            firstFrame = true;
            float timeoutStartTime = 0;

            while (IsStreaming)
            {
                bool frameGot = false;
                bool streamLost = false;
                try
                {
                    float elapsedTime = 0;
                    int bytesToRead = -1;
                    try
                    {
                        bytesToRead = FindLength(stream);
                    }
#if UNITY_WSA_10_0 && WINDOWS_UWP
                    catch (IOException)
#else
                    catch (WebException)
#endif
                    {
                    }
                    catch (ObjectDisposedException)
                    {
                        Debug.LogError("Stream lost while reading header.");
                        streamLost = true;
                    }
                    if (streamLost)
                    {
                        yield return OnStreamLost();
                        yield break;
                    }
                    if (bytesToRead == -1)
                    {
                        if (timeoutStartTime == 0)
                        {
                            timeoutStartTime = Time.realtimeSinceStartup;
                        }
                        Debug.Log(thisClassName + ": waiting for frames.");
                        //                print("End of stream");
                        yield return null;
                        elapsedTime += Time.realtimeSinceStartup - timeoutStartTime;
                        if (elapsedTime < giveUpTimeout)
                        {
                            continue;
                        }
                        else
                        {
                            timeoutStartTime = 0;
                            Debug.LogErrorFormat("Connection from IP camera lost after {F1} seconds.", elapsedTime);
                            yield return OnStreamLost();
                            yield break;
                        }
                    }
//#if UNITY_WSA_10_0 && WINDOWS_UWP
//                if (jpegData.Length != bytesToRead)
//                {
//                    jpegData = new byte[bytesToRead];
//                }
//#else
                    if (jpegData.Length < bytesToRead)
                    {
                        jpegData = new byte[bytesToRead * 2];
                    }
//#endif

                    int leftToRead = bytesToRead;

                    while (leftToRead > 0 && IsStreaming)
                    {
                        //print(leftToRead);
                        try
                        {
                            leftToRead -= stream.Read(
                            jpegData, bytesToRead - leftToRead, leftToRead);

                        }
#if UNITY_WSA_10_0 && WINDOWS_UWP
                        catch (IOException)
#else
                        catch (WebException)
#endif
                        {
                            //if(e.Message != "The operation has timed out.")
                            //{
                            //    throw e;
                            //}
                        }
                        catch (ObjectDisposedException)
                        {
                            streamLost = true;
                        }
                        if (streamLost)
                        {
                            Debug.LogError("Stream lost.");
                            yield return OnStreamLost();
                            yield break;
                        }
                        yield return null;
                    }
                    if (IsStreaming)
                    {
                        stream.ReadByte(); // CR after bytes
                        stream.ReadByte(); // LF after bytes
                                        //Debug.Log(thisClassName + ": Frame read");
//#if UNITY_WSA_10_0 && WINDOWS_UWP
					texture.LoadImage(jpegData);
//#else
//                    MemoryStream ms = new MemoryStream(jpegData, 0, bytesToRead, false, true);
//                    texture.LoadImage(ms.GetBuffer());
//#endif
                        if (savedTexWidth != texture.width || savedTexHeight != texture.height)
                        {
                            savedTexWidth = texture.width;
                            savedTexHeight = texture.height;
                            resizeFrameImage = true;
                        }
                        frameGot = true;
                        OnConnectionSucceeded();
                        //if (frame != null)
                        //{
                        //    frame.material.mainTexture = texture;
                        //}
                        //yield return null;
                        //frame.material.color = Color.white;
                        //yield return null;
                        firstFrame = false;
                    }
                }
                finally
                {
                    if (IsStreaming && !frameGot)
                    {
                        Debug.LogError("Frame missed.");
                    }
                }
            }
        }

#if UNITY_WSA_10_0 && WINDOWS_UWP
        private IEnumerator GetVideoStream()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(readTimeout);
                Stream stream = httpClient.GetStreamAsync(url).Result;

                using (StreamReader reader = new StreamReader(stream))
                {
                    yield return GetFrames(stream);
                }
            }
            Debug.Log("Stream closed.");
        }
#else
        private IEnumerator GetVideoStream()
        {
            Stream stream;
            WebResponse resp;
            restartStream = false;
            // create HTTP request
            HttpWebRequest req = null;

            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (FormatException e)
            {
                OnConnectionFailed();
                Debug.LogErrorFormat("Wrong format for IP camera URL: {0}", e);
                yield break;
            }

            try
            {
                if (!string.IsNullOrEmpty(login))
                {
                    req.Credentials = new NetworkCredential(login, password);
                }
                // get response
                resp = req.GetResponse();
            }
#if UNITY_WSA_10_0 && WINDOWS_UWP
            catch (IOException e)
#else
            catch (WebException e)
#endif
            {
                OnConnectionFailed();
                Debug.LogErrorFormat("Failed to connect to IP camera: {0}", e);
                yield break;
            }

            // get response stream
            using (stream = resp.GetResponseStream())
            {
                yield return GetFrames(stream);
            }
            Debug.Log("Stream closed.");
            //StopStream();
        }
#endif

        private string ReadHeader(Stream stream)
        {
            int b = 0;
            bool atEndOfHeader = false;
            bool headerComplete = false;
            StringBuilder headerBuilder = new StringBuilder();
            while (IsStreaming && !headerComplete)
            {
                b = stream.ReadByte();
                if (b == -1)
                {
                    return null;
                }
                headerBuilder.Append((char)b);
                if (b != 10)
                {
                    if (b == 13)
                    {
                        if (atEndOfHeader)
                        {
                            headerBuilder.Append((char)stream.ReadByte());
                            headerComplete = true;
                        }
                        atEndOfHeader = true;
                    }
                    else
                    {
                        atEndOfHeader = false;
                    }
                }
            }
            return headerBuilder.ToString();
        }

        private string ExtractHeaderValue(string header, string tag, int startIdx)
        {
            int tagLength = tag.Length;
            int tagIdx = header.IndexOf(tag, startIdx);
            if (tagIdx == -1)
            {
                return null;
            }
            int nlIdx = header.IndexOf((char)13, tagIdx + tagLength);
            if (nlIdx == -1)
            {
                return null;
            }
            int valIdx = tagIdx + tagLength;
            string valString = header.Substring(valIdx, nlIdx - valIdx).Trim();
            return valString;
        }

        private int FindLength(Stream stream)
        {
            int result = -1;
            const string typeTag = "Content-Type:";
            const string lengthTag = "Content-Length:";
            int lengthTagLength = lengthTag.Length;

            string header = ReadHeader(stream);
            if (header == null)
            {
                return -1;
            }
            if (firstFrame)
            {
                Debug.LogFormat("{0} - frame header read: {1}", thisClassName, header);
            }
            result = Convert.ToInt32(ExtractHeaderValue(header, lengthTag, 0));
            string contentType = ExtractHeaderValue(header, typeTag, 0);
            if (contentType != "image/jpeg")
            {
                IsStreaming = false;
                Debug.LogErrorFormat("{0} - Content type non supported: {1}", thisClassName, contentType);
            }
            return result;
        }

        void AdjustImageSize()
        {
            if (texture != null)
            {
                Debug.LogFormat("Size: {0},{1}", texture.width, texture.height);
                float aspect = texture.width / (float)texture.height;
                if (frameImage != null && aspect > 0)
                {
                    if (aspect > 1f)
                    {
                        frameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frameImage.canvas.pixelRect.width);
                        frameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, frameImage.canvas.pixelRect.width / aspect);
                    }
                    else
                    {
                        frameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, frameImage.canvas.pixelRect.height * aspect);
                        frameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frameImage.canvas.pixelRect.height);
                    }
                }
                resizeFrameImage = false;
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            AdjustImageSize();
        }

        private void Awake()
        {
            IsStreaming = false;
            thisClassName = GetType().Name;
            texture = new Texture2D(2, 2);
            jpegData = new byte[1024000];//1M
        }

        private void Start()
        {
            savedWidth = Screen.width;
            savedHeight = Screen.height;
            if (autoStartStreaming)
            {
                StartStream();
            }
        }

        private void OnDisable()
        {
            StopStream();
        }

        private void Update()
        {
            if (restartStream)
            {
                StartStream();
            }
            if (resizeFrameImage || savedWidth != Screen.width || savedHeight != Screen.height)
            {
                savedWidth = Screen.width;
                savedHeight = Screen.height;

                AdjustImageSize();
            }
        }
    }
}