using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK.Video
{
    /// <summary>
    /// Acquire images from a video stream coming from a local webcam
    /// </summary>
    [RequireComponent(typeof(WebcamComponent))]
    public class WebcamToMaterial : VideoToMaterial
    {
        private WebcamComponent webCamComponent;

        /// <summary>
        /// The webcam is playing (read only).
        /// </summary>
        public override bool Playing
        {
            get
            {
                return webCamComponent.Playing;
            }
        }

        /// <summary>
        /// The webcam is paused (read only).
        /// </summary>
        public override bool Paused
        {
            get
            {
                return webCamComponent.Paused;
            }
        }

        /// <summary>
        /// The webcam is initialized (read only).
        /// </summary>
        public override bool Initialized
        {
            get
            {
                return webCamComponent.Initialized;
            }
        }

        /// <summary>
        /// The name of the webcam device (read only).
        /// </summary>
        public string DeviceName
        {
            get
            {
                return webCamComponent.DeviceName;
            }
        }

        protected override int FrameWidth
        {
            get
            {
                return webCamComponent.FrameWidth;
            }
        }

        protected override int FrameHeight
        {
            get
            {
                return webCamComponent.FrameHeight;
            }
        }

        protected override Texture FrameTexture
        {
            get
            {
                return webCamComponent.FrameTexture;
            }
        }

        protected override void PlayVideo()
        {
            webCamComponent.Play();
        }

        protected override void PauseVideo()
        {
            webCamComponent.Pause();
        }

        protected override void StopVideo()
        {
            webCamComponent.Stop();
        }

        protected override bool InitializeVideo()
        {
            return webCamComponent.Initialize();
        }

        protected override void TerminateVideo()
        {
            webCamComponent.Terminate();
        }

        private void Awake()
        {
            webCamComponent = GetComponent<WebcamComponent>();
        }
    }
}
