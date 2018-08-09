using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    public class SwitchObject : MonoBehaviour
    {

        public GameObject[] switchedObjects;
        public int startIndex = 0;
        public Image progressImage;
        public Text idText;
        public Text infoText;
        public AudioSource audioSource;
        private int currIndex = 0;

        public bool IsDefined
        {
            get
            {
                return (switchedObjects != null || switchedObjects.Length > 0);
            }
        }

        public int CurrIndex { get { return currIndex; } }

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
                if (objInfo.autoRead)
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

        // Use this for initialization
        void Start()
        {
            Switch(startIndex);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
