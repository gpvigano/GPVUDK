using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// Controls the switch of game objects by index or name or in sequence from a given list.
    /// </summary>
    public class SwitchObject : MonoBehaviour
    {
        [Tooltip("Index in the list of the object to be switched on first.")]
        public int startIndex = 0;
        [Tooltip("Image script of the progress bar used to show the current position in the list.")]
        public Image progressImage;
        [Tooltip("Text box used to display the identifier of the object (from its Object Info component)")]
        public Text idText;
        [Tooltip("Text box used to display the description of the object (from its Object Info component)")]
        public Text infoText;
        [Tooltip("Text box used to play the audio clip of the object (from its Object Info component)")]
        public AudioSource audioSource;
        [Tooltip("List of game object to be switched in sequence")]
        public GameObject[] switchedObjects;

        private int currIndex = 0;

        /// <summary>
        /// This is true only if at least one element is in the list.
        /// </summary>
        public bool IsDefined
        {
            get
            {
                return (switchedObjects != null || switchedObjects.Length > 0);
            }
        }

        /// <summary>
        /// The index of the currently active element in the list.
        /// </summary>
        public int CurrIndex { get { return currIndex; } }

        /// <summary>
        /// Switch to the first object with the given name.
        /// </summary>
        /// <param name="objectName">name of the element in the list to be switched on.</param>
        public void Switch(string objectName)
        {
            if (!IsDefined)
            {
                return;
            }
            for (int i = 0; i < switchedObjects.Length; i++)
            {
                if (switchedObjects[i].name == objectName)
                {
                    Switch(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Switch to the object with the given index.
        /// </summary>
        /// <param name="index">index of the element in the list to be switched on.</param>
        public void Switch(int index)
        {
            if (!IsDefined)
            {
                return;
            }
            currIndex = index;
            ObjectInfo objInfo = null;
            for (int i = 0; i < switchedObjects.Length; i++)
            {
                switchedObjects[i].SetActive(i == index);
                if (i == index)
                {
                    GameObject activeObject = switchedObjects[i];
                    objInfo = activeObject.GetComponent<ObjectInfo>();
                }
            }
            if (progressImage != null)
            {
                progressImage.fillAmount = index / (float)(switchedObjects.Length - 1);
            }
            if (objInfo != null)
            {
                if (idText != null)
                {
                    idText.text = objInfo.identifier;
                }
                if (infoText != null)
                {
                    infoText.text = objInfo.description;
                }
                if (objInfo.autoSay)
                {
                    Speak();
                }
            }
            else
            {
                if (idText != null)
                {
                    idText.text = "";
                }
                if (infoText != null)
                {
                    infoText.text = "";
                }
            }
        }

        /// <summary>
        /// Switch to the next element in the list,
        /// if at last position restart from the beginning.
        /// </summary>
        public void CycleSwitchForward()
        {
            if (!IsDefined)
            {
                return;
            }
            currIndex++;
            if (currIndex >= switchedObjects.Length)
            {
                currIndex = 0;
            }
            Switch(currIndex);
        }

        /// <summary>
        /// Switch to the previous element in the list,
        /// if at first position restart from the end.
        /// </summary>
        public void CycleSwitchBackward()
        {
            if (!IsDefined)
            {
                return;
            }
            currIndex--;
            if (currIndex < 0)
            {
                currIndex = switchedObjects.Length - 1;
            }
            Switch(currIndex);
        }

        /// <summary>
        /// Play the audio clip of the Object Info component  (if any).
        /// </summary>
        public void Speak()
        {
            if (!IsDefined || currIndex < 0 || currIndex >= switchedObjects.Length)
            {
                return;
            }
            GameObject activeObject = switchedObjects[currIndex];
            if (activeObject != null)
            {
                ObjectInfo objInfo = activeObject.GetComponent<ObjectInfo>();
                if (objInfo != null)
                {
                    AudioClip audioClip = objInfo.audioClip;
                    if (audioSource != null && audioClip != null)
                    {
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }
                }
            }
        }

        private void Start()
        {
            // setup the initial configuration
            Switch(startIndex);
        }

        private void Update()
        {
        }
    }
}
