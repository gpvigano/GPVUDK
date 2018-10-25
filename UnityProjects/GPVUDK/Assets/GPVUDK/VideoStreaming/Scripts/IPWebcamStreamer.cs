// code inspired by:
// https://answers.unity.com/questions/1245582/create-a-simple-http-server-on-the-streaming-asset.html
// and:
// http://www.ridgesolutions.ie/index.php/2014/11/24/streaming-mjpeg-video-with-web2py-and-python/

using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using UnityEngine;

namespace GPVUDK.Video
{
    /// <summary>
    /// Stream frames from a webcam over Internet or LAN.
    /// </summary>
    public class IPWebcamStreamer : MonoBehaviour
    {
        [Tooltip("Port used to access this HTTP server (0=first free)")]
        [SerializeField]
        private int serverPort = 0;
        [Tooltip("Automatically start video streaming on startup")]
        [SerializeField]
        private bool autoStart = false;
        [Tooltip("Send only a frame for each request")]
        [SerializeField]
        private bool singleFrame = false;
        [SerializeField]
        private WebcamComponent webCamComponent;

        private HttpFrameServer frameServer = new HttpFrameServer();
        private bool isStreaming = false;

        /// <summary>
        /// Port used for listening, it cannot be set while listening (0 = use the first free)
        /// </summary>
        public int ServerPort
        {
            get
            {
                if (frameServer.Listening)
                {
                    return frameServer.ServerPort;
                }
                else
                {
                    return serverPort;
                }
            }

            set
            {
                if (frameServer.Listening)
                {
                    Debug.LogWarning("Cannot change the port while frame server is running.");
                }
                else
                {
                    serverPort = value;
                }
            }
        }

        /// <summary>
        /// Event fired when started streming
        /// </summary>
        public event Action StreamingStarted;
        /// <summary>
        /// Event fired when stopped streming
        /// </summary>
        public event Action StreamingStopped;

        /// <summary>
        /// Start the webcam and video streaming.
        /// </summary>
        public void StartServer()
        {
            if (webCamComponent.Initialize())
            {
                webCamComponent.Play();
            }
            //StartCoroutine(StreamVideo());
            frameServer.Start(serverPort, singleFrame);
            Application.runInBackground = true;
            StartStreaming();
            if (StreamingStarted != null)
            {
                StreamingStarted();
            }
        }

        /// <summary>
        /// Pause video streaming over IP
        /// </summary>
        public void PauseStreaming()
        {
            if (isStreaming)
            {
                isStreaming = false;
            }
        }

        /// <summary>
        /// Start/resume video streaming over IP
        /// </summary>
        public void StartStreaming()
        {
            if (!isStreaming && frameServer.Listening)
            {
                isStreaming = true;
                StartCoroutine(StreamVideo());
            }
        }

        /// <summary>
        /// Stop the video frame server and the webcam.
        /// </summary>
        public void StopServer()
        {
            if (frameServer.Listening)
            {
                frameServer.Stop();
            }
            isStreaming = false;
            if (webCamComponent != null)
            {
                webCamComponent.Stop();
            }
            if (StreamingStopped != null)
            {
                StreamingStopped();
            }
        }

        private IEnumerator StreamVideo()
        {
            while (!webCamComponent.Playing)
            {
                yield return null;
            }
            while (webCamComponent.Playing && isStreaming)
            {
                if (frameServer.Listening)
                {
                    frameServer.UpdateImageBuffer(webCamComponent.GetJPG());
                }
                yield return null;
            }
            if (frameServer.Listening)
            {
                frameServer.UpdateImageBuffer(null);
            }
            print("End of video streaming.");
        }

        private void Awake()
        {
            if (webCamComponent == null)
            {
                webCamComponent = GetComponent<WebcamComponent>();
            }
        }

        private void Start()
        {
            if (autoStart)
            {
                StartServer();
            }
        }

        private void OnDisable()
        {
            Quit();
        }

        private void OnApplicationQuit()
        {
            Quit();
        }

        private void OnDestroy()
        {
            StopServer();
        }

        private void Quit()
        {
            StopServer();
            webCamComponent.Stop();
            webCamComponent.Terminate();
        }

        /// <summary>
        /// HTTP frame server management.
        /// </summary>
        private class HttpFrameServer
        {
            private byte[] imageBuffer = null;
            private bool singleFrame = false;
            private Thread serverThread = null;
            private HttpListener httpListener = null;
            private HttpListenerContext context = null;
            private readonly object bufferLock = new object();
            private bool isListening = false;

            public int ServerPort { get; private set; }

            public bool SingleFrame
            {
                get
                {
                    return singleFrame;
                }

                set
                {
                    singleFrame = value;
                }
            }

            public bool Listening
            {
                get
                {
                    return isListening;
                }
            }

            /// <summary>
            /// Construct server with given port.
            /// </summary>
            public HttpFrameServer()
            {
            }

            /// <summary>
            /// Construct server with given port or the first free if port is 0.
            /// </summary>
            /// <param name="port">Port of the server.</param>
            /// <param name="singleFrameMode">Send a single image for each request</param>
            public HttpFrameServer(int port, bool singleFrameMode)
            {
                this.singleFrame = singleFrameMode;
                this.StartListening(port);
            }

            /// <summary>
            /// Start the server with given port or the first free if port is 0.
            /// </summary>
            /// <param name="port">Port of the server.</param>
            /// <param name="singleFrameMode">Send a single image for each request.</param>
            public void Start(int port, bool singleFrameMode)
            {
                if (serverThread == null || serverThread.ThreadState != ThreadState.Running)
                {
                    singleFrame = singleFrameMode;
                    StartListening(port);
                }
            }

            /// <summary>
            /// Update the frame image buffer with a new image (thread safe).
            /// </summary>
            /// <param name="buffer">Frame image buffer (JPEG image bytes)</param>
            public void UpdateImageBuffer(byte[] buffer)
            {
                lock (bufferLock)
                {
                    imageBuffer = buffer;
                }
            }

            /// <summary>
            /// Stop server and make the thread stop.
            /// </summary>
            public void Stop()
            {
                if (isListening && httpListener != null)
                {
                    UpdateImageBuffer(null);
                    // TODO: this could be a race condition
                    TerminateListener();
                    //try
                    //{
                    //    if (serverThread.IsAlive)
                    //    {
                    //        serverThread.Abort();
                    //        Debug.Log("Trying to abort");
                    //    }
                    //}
                    //catch
                    //{
                    //    Debug.Log("Aborting thread failed");
                    //}
                    print("Frame server stopped.");
                }
            }

            private void Listen()
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add("http://*:" + ServerPort.ToString() + "/");
                httpListener.Start();
                print("Listening...");
                try
                {
                    while (httpListener != null && httpListener.IsListening)
                    {
                        context = httpListener.GetContext();
                        print("Request from " + context.Request.Url);
                        try
                        {
                            Process(context);
                        }
                        catch (IOException e)
                        {
                            print(e.Message);
                            httpListener.Stop();
                            httpListener.Start();
                        }
                    }
                }
                catch (Exception e)
                {
                    print(e);
                }
                finally
                {
                    TerminateListener();
                }
            }

            private void TerminateListener()
            {
                if (httpListener != null)
                {
                    if (context != null)
                    {
                        context.Response.OutputStream.Close();
                        context.Response.Close();
                    }
                    httpListener.Close();
                    httpListener = null;
                    isListening = false;
                    print("Stopped listening.");
                }
            }

            private void WriteToStream(Stream stream, string msg)
            {
                byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
                stream.Write(msgBytes, 0, msgBytes.Length);
            }

            private void Process(HttpListenerContext context)
            {
                byte[] localBuffer = null;

                // wait until a frame is available
                do
                {
                    if (!isListening)
                    {
                        // quit if the server has been stopped
                        return;
                    }
                    Thread.Sleep(0);
                    lock (bufferLock)
                    {
                        localBuffer = imageBuffer;
                    }
                } while (localBuffer == null);

                try
                {
                    // send the current frame
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    if (singleFrame)
                    {
                        lock (bufferLock)
                        {
                            int nbytes = imageBuffer.Length;
                            // Adding permanent http response headers
                            context.Response.ContentType = "image/jpeg";
                            context.Response.ContentLength64 = nbytes;
                            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                            context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                            context.Response.OutputStream.Write(imageBuffer, 0, nbytes);
                            context.Response.OutputStream.Flush();
                        }
                    }
                    else
                    {
                        context.Response.ContentType = "multipart/x-mixed-replace; boundary=unity_gpvudk";// _mimeTypeMappings.TryGetValue(Path.GetExtension(absolutePath), out mime) ? mime : "application/octet-stream";
                        do
                        {
                            lock (bufferLock)
                            {
                                if (localBuffer != imageBuffer && imageBuffer != null)
                                {
                                    localBuffer = imageBuffer;
                                    WriteToStream(context.Response.OutputStream, "--unity_gpvudk\r\n");
                                    WriteToStream(context.Response.OutputStream, "Content-Type: image/jpeg\r\n");
                                    int nbytes = imageBuffer.Length;
                                    WriteToStream(context.Response.OutputStream, "Content-Length: " + nbytes + "\r\n\r\n");
                                    context.Response.OutputStream.Write(imageBuffer, 0, nbytes);
                                    WriteToStream(context.Response.OutputStream, "\r\n");
                                    context.Response.OutputStream.Flush();
                                }
                            }
                            Thread.Sleep(0);
                        } while (localBuffer != null);
                    }
                }
                finally
                {
                    context.Response.OutputStream.Close();
                    if (!singleFrame)
                    {
                        print("Streaming stopped.");
                    }
                }
            }


            private void StartListening(int port)
            {
                ServerPort = port;
                if (port == 0)
                {
                    //get an empty port
                    TcpListener tempListener = new TcpListener(IPAddress.Loopback, 0);
                    tempListener.Start();
                    ServerPort = ((IPEndPoint)tempListener.LocalEndpoint).Port;
                    tempListener.Stop();
                    print("Listening on port: " + ServerPort);
                }
                serverThread = new Thread(this.Listen);
                serverThread.Start();
                isListening = true;
                print("Frame server started.");
            }
        }
    }
}
