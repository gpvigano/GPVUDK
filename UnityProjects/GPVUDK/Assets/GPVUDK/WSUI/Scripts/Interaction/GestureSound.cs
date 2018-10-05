using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Play a sound when a gesture is detected (if audioClip is set)
    /// </summary>
    [RequireComponent(typeof(GestureDetector))]
    public class GestureSound : MonoBehaviour
    {
        [Tooltip("An audio source is needed to play sounds (None = get it from this object).")]
        public AudioSource audioSource;
        [Tooltip("Audio clip for \"tap\" gesture sound (optional).")]
        public AudioClip tapAudioClip;
        [Tooltip("Audio clip for \"interaction start\" gesture sound (optional).")]
        public AudioClip interactStartAudioClip;
        [Tooltip("Audio clip for \"interaction end\" gesture sound (optional).")]
        public AudioClip interactEndAudioClip;

        private GestureDetector gestureDetector;

        private void Awake()
        {
            gestureDetector = GetComponent<GestureDetector>();
            if(audioSource==null )
            {
                audioSource = GetComponent<AudioSource>();
            }
            if(audioSource==null )
            {
                Debug.LogError(GetType()+" cannot work without an Audio Sourece defined!");
            }
        }

        private void OnEnable()
        {
            // Register to gesture events
            gestureDetector.Tap += OnTapped;
            gestureDetector.Activate += OnStartInteracting;
            gestureDetector.Deactivate += OnStopInteracting;
        }

        private void OnDisable()
        {
            // Unregister from tap events
            gestureDetector.Tap -= OnTapped;
            gestureDetector.Activate -= OnStartInteracting;
            gestureDetector.Deactivate -= OnStopInteracting;
        }

        private void OnTapped()
        {
            PlaySound(tapAudioClip);
        }

        private void OnStartInteracting()
        {
            PlaySound(interactStartAudioClip);
        }

        private void OnStopInteracting()
        {
            PlaySound(interactEndAudioClip);
        }

        private void PlaySound(AudioClip audioClip)
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }
}
