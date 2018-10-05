using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    public class AppQuitManager : MonoBehaviour
    {
        [Tooltip("Time left for cancelling the application quit")]
        public float quitTimeout = 10f;
        [Tooltip("Panel with the Image Type set to Filled")]
        public Image progressImage;
        [Tooltip("Audio source for events")]
        public AudioSource audioSource;
        [Tooltip("Sound played on application quit")]
        public AudioClip quitSound;
        [Tooltip("Sound played on application quit cancelled")]
        public AudioClip cancelSound;
        [Tooltip("Objects to show while waiting for cancelling")]
        public GameObject[] objectsToShowOnConfirmation;
        [Tooltip("Objects to hide while waiting for cancelling")]
        public GameObject[] objectsToHideOnConfirmation;

        private GestureDetector tapDetector;
        private List<GameObject> objectsToHide = new List<GameObject>();
        private float quitTime = 0;

        public void QuitNow()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public virtual void QuitApplication()
        {
            objectsToHide.Clear();
            foreach (GameObject obj in objectsToHideOnConfirmation)
            {
                if (obj.activeSelf)
                {
                    objectsToHide.Add(obj);
                    obj.SetActive(false);
                }
            }
            foreach (GameObject obj in objectsToShowOnConfirmation)
            {
                obj.SetActive(true);
            }
            quitTime = Time.realtimeSinceStartup;
            if (audioSource != null && quitSound != null)
            {
                audioSource.clip = quitSound;
                audioSource.Play();
            }
            if (tapDetector != null)
            {
                tapDetector.Tap += CancelQuit;
            }
        }

        public virtual void CancelQuit()
        {
            tapDetector.Tap -= CancelQuit;
            if (quitTime > 0)
            {
                foreach (GameObject obj in objectsToHide)
                {
                    obj.SetActive(true);
                }
                foreach (GameObject obj in objectsToShowOnConfirmation)
                {
                    obj.SetActive(false);
                }
                quitTime = 0;
                if (audioSource != null && cancelSound != null)
                {
                    audioSource.clip = cancelSound;
                    audioSource.Play();
                }
            }
        }

        public virtual void ConfirmQuit()
        {
            if (tapDetector != null)
            {
                tapDetector.Tap -= CancelQuit;
            }
            QuitNow();
        }

        private void Awake()
        {
            tapDetector = FindObjectOfType<GestureDetector>();
        }

        private void Update()
        {
            if (quitTime > 0)
            {
                float elapsedTime = Time.realtimeSinceStartup - quitTime;
                if (progressImage != null)
                {
                    progressImage.fillAmount = elapsedTime / quitTimeout;
                }

                if (elapsedTime > quitTimeout)
                {
                    ConfirmQuit();
                }
            }
        }
    }
}
