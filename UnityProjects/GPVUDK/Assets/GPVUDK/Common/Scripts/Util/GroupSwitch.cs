using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class GroupSwitch : MonoBehaviour
    {
        public bool initiallyOn = true;
        public GameObject[] switchedObjects;

        private bool isOn = false;
        private Dictionary<GameObject, bool> savedStates = new Dictionary<GameObject, bool>();
        private bool isOnInitialized = false;

        public bool IsOn
        {
            get
            {
                return isOnInitialized?isOn:initiallyOn;
            }

            set
            {
                Switch(value);
            }
        }

        public void Switch(bool on)
        {
            if(isOnInitialized&&(switchedObjects==null || isOn==on))
            {
                return;
            }
            if(!isOnInitialized)
            {
                SaveObjectStates();
            }
            isOn = on;
            if(on)
            {
                foreach (GameObject obj in switchedObjects)
                {
                    obj.SetActive(savedStates[obj]);
                }
            }
            else
            {
                SaveObjectStates();
                foreach (GameObject obj in switchedObjects)
                {
                    obj.SetActive(false);
                }
            }
            isOnInitialized = true;
        }

        private void SaveObjectStates()
        {
                savedStates.Clear();
                foreach (GameObject obj in switchedObjects)
                {
                    savedStates.Add(obj, obj.activeSelf);
                }
        }

        private void Awake()
        {
            isOn = initiallyOn;           
        }

        private void Start()
        {
            Switch(isOn);
        }
    }
}

