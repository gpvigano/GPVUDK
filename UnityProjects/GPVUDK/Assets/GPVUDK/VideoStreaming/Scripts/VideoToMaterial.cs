using System;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK.Video
{
    /// <summary>
    /// Abstract class for video rendering to a material (Image or Renderer).
    /// </summary>
    public abstract class VideoToMaterial : MonoBehaviour
    {
        [Tooltip("Start video on startup")]
        [SerializeField]
        protected bool autoStartVideo = true;
        [Tooltip("Directly change materials instead of using their copies.")]
        [SerializeField]
        protected bool sharedMaterials = false;
        [Tooltip("Show the image game object when playing (or paused) and hide it when stopped.")]
        [SerializeField]
        protected bool toggleImage = false;
        [Tooltip("Image with the material texture for the frames")]
        [SerializeField]
        private Image frameImage;
        [Tooltip("Image resizer for the above image")]
        [SerializeField]
        private FrameImageResizer imageResizer;
        [Tooltip("Renderer with the material texture for the frames")]
        [SerializeField]
        private Renderer frameRenderer;

        private Material rendererMaterial = null;
        private Material imageMaterial = null;

        /// <summary>
        /// To be defined for playing state (read only).
        /// </summary>
        public abstract bool Playing { get; }
        /// <summary>
        /// To be defined for pausing state (read only).
        /// </summary>
        public abstract bool Paused { get; }
        /// <summary>
        /// To be defined for initialization state (read only).
        /// </summary>
        public abstract bool Initialized { get; }

        /// <summary>
        /// Texture of the frame buffer built by the video implementation.
        /// </summary>
        protected abstract Texture FrameTexture { get; }
        /// <summary>
        /// Width of the frame buffer, defined by the video implementation.
        /// </summary>
        protected abstract int FrameWidth { get; }
        /// <summary>
        /// Height of the frame buffer, defined by the video implementation.
        /// </summary>
        protected abstract int FrameHeight { get; }

        /// <summary>
        /// Event triggered after initialization.
        /// </summary>
        public event Action InitializedEvent;
        /// <summary>
        /// Event triggered when Play() is called.
        /// </summary>
        public event Action PlayEvent;
        /// <summary>
        /// Event triggered when Pause() is called.
        /// </summary>
        public event Action PauseEvent;
        /// <summary>
        /// Event triggered when Stop() is called.
        /// </summary>
        public event Action StopEvent;

        /// <summary>
        /// Play frames from the video source (initialize if not yet initialized).
        /// </summary>
        public void Play()
        {
            if (!Initialized)
            {
                Initialize();
            }
            if (Initialized)
            {
                PlayVideo();
                if (imageResizer != null)
                {
                    imageResizer.SwitchResizing(true);
                }
                if (frameImage != null)
                {
                    // show the frame image in case it was hidden
                    frameImage.gameObject.SetActive(true);
                }
                if (PlayEvent != null)
                {
                    PlayEvent();
                }
            }
        }

        /// <summary>
        /// Pause the video if playing.
        /// </summary>
        public void Pause()
        {
            if (Initialized && Playing && !Paused)
            {
                PauseVideo();
                if (PauseEvent != null)
                {
                    PauseEvent();
                }
            }
        }

        /// <summary>
        /// Stop the video if playing or paused.
        /// </summary>
        public void Stop()
        {
            if (Initialized && (Playing || Paused))
            {
                StopVideo();
                if (imageResizer != null)
                {
                    imageResizer.SwitchResizing(false);
                }
                if (toggleImage && frameImage != null)
                {
                    frameImage.gameObject.SetActive(false);
                }
                if (StopEvent != null)
                {
                    StopEvent();
                }
            }
        }

        /// <summary>
        /// Abstract method to be overridden for video initialization.
        /// </summary>
        /// <returns>It must return true on success, false on failure.</returns>
        protected abstract bool InitializeVideo();
        /// <summary>
        /// Abstract method to be overridden for terminating the video (free resources).
        /// </summary>
        protected abstract void TerminateVideo();

        /// <summary>
        /// Abstract method to be overridden for playing the video from the implemented source.
        /// </summary>
        protected abstract void PlayVideo();
        /// <summary>
        /// Abstract method to be overridden for pausing the video.
        /// </summary>
        protected abstract void PauseVideo();
        /// <summary>
        /// Abstract method to be overridden for stopping the video.
        /// </summary>
        protected abstract void StopVideo();

        /// <summary>
        /// Initialize calling InitializeVideo(), InitializeMaterials(), OnInitialize().
        /// </summary>
        /// <remarks>Usually it is enough to override InitializeVideo() and TerminateVideo()</remarks>
        /// <returns>It returns true on success, false on failure.</returns>
        protected virtual bool Initialize()
        {
            if (!InitializeVideo())
            {
                return false;
            }
            InitializeMaterials();
            OnInitialize();
            return true;
        }

        /// <summary>
        /// Trigger the InitializedEvent event.
        /// </summary>
        protected virtual void OnInitialize()
        {
            if (InitializedEvent != null)
            {
                InitializedEvent();
            }
        }

        /// <summary>
        /// Terminate video calling TerminateVideo() and reset materials.
        /// </summary>
        protected virtual void Terminate()
        {
            if (Initialized)
            {
                if (Playing || Paused)
                {
                    StopVideo();
                }
                TerminateVideo();
                if (rendererMaterial != null)
                {
                    rendererMaterial.mainTexture = null;
                }
                if (imageMaterial != null)
                {
                    imageMaterial.mainTexture = null;
                }
            }
        }

        /// <summary>
        /// Play the video on startup if autoStartVideo is true.
        /// </summary>
        protected virtual void Start()
        {
            if (autoStartVideo)
            {
                Play();
            }
        }

        /// <summary>
        /// Call Teminate() on application quit.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            Terminate();
        }

        /// <summary>
        /// On enalble play the video if autoStartVideo is true.
        /// </summary>
        protected virtual void OnEnable()
        {
            // OnEnable() is first called on scene load,
            // then the video is played by Start().
            // Here we play it only if it was disabled
            // after Start()
            if (Initialized && !Playing && autoStartVideo)
            {
                PlayVideo();
            }
        }

        /// <summary>
        /// On enalble stop the video.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (Playing || Paused)
            {
                StopVideo();
            }
        }

        /// <summary>
        /// Call Teminate() on destroy.
        /// </summary>
        protected virtual void OnDestroy()
        {
            TerminateVideo();
        }

        /// <summary>
        /// Add or get a FrameImageResizer and set its imageResizer if frameImage is defined.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (frameImage != null)
            {
                if (imageResizer == null)
                {
                    imageResizer = GetComponent<FrameImageResizer>();
                    if (imageResizer == null)
                    {
                        imageResizer = gameObject.AddComponent<FrameImageResizer>();
                    }
                }
                imageResizer.SetFrameImage(frameImage);
            }
        }

        /// <summary>
        /// Update the image transform according to the frame size.
        /// </summary>
        protected virtual void Update()
        {
            Texture frameTexture = FrameTexture;
            if (rendererMaterial != null && rendererMaterial.mainTexture != frameTexture)
            {
                rendererMaterial.mainTexture = frameTexture;
            }
            if (imageMaterial != null && imageMaterial.mainTexture != frameTexture)
            {
                imageMaterial.mainTexture = frameTexture;
            }
            if (imageResizer != null && Initialized)
            {
                imageResizer.UpdateFrameSize(FrameWidth, FrameHeight);
            }
        }

        /// <summary>
        /// Initialize the materials duplicating the original ones if sharedMaterials is false.
        /// </summary>
        private void InitializeMaterials()
        {
            if (frameRenderer != null && rendererMaterial == null)
            {
                if (sharedMaterials && frameRenderer.sharedMaterial != null)
                {
                    rendererMaterial = frameRenderer.sharedMaterial;
                }
                else
                {
                    rendererMaterial = new Material(frameRenderer.material);
                    frameRenderer.sharedMaterial = rendererMaterial;
                }
                rendererMaterial.mainTexture = FrameTexture;
            }
            if (frameImage != null && imageMaterial == null)
            {
                if (sharedMaterials && frameImage.material != null)
                {
                    imageMaterial = frameImage.material;
                }
                else
                {
                    imageMaterial = new Material(frameImage.material);
                    frameImage.material = imageMaterial;
                }
                imageMaterial.mainTexture = FrameTexture;
            }

        }
    }
}
