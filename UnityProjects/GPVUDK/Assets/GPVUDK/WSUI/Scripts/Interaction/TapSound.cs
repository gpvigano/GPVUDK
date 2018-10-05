using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Play a sound when a tap gesture is detected (if audioClip is set)
    /// </summary>
    [RequireComponent(typeof(GestureDetector))]
    public class TapSound : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip audioClip;

        private GestureDetector gestureDetector;

        private void Awake()
        {
            gestureDetector = GetComponent<GestureDetector>();
        }

        private void OnEnable()
        {
            // Register to tap events
            gestureDetector.Tap += OnTapped;
        }

        private void OnDisable()
        {
            // Unregister from tap events
            gestureDetector.Tap -= OnTapped;
        }

        // Update is called once per frame
        void OnTapped()
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
