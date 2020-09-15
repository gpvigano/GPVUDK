using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Switch a group of objects on or off.
    /// </summary>
    public class GroupSwitch : MonoBehaviour
    {
        /// <summary>
        /// The initial state: on (checked) or off.
        /// </summary>
        [Tooltip("The initial state: on (checked) or off")]
        public bool initiallyOn = true;
		
        /// <summary>
        /// List of objects to be switched together.
        /// </summary>
        [Tooltip("List of objects to be switched together")]
        public GameObject[] switchedObjects;

        private bool isOn = false;
        private Dictionary<GameObject, bool> savedStates = new Dictionary<GameObject, bool>();
        private bool isOnInitialized = false;

		
        /// <summary>
        /// The group of object is on (not necessarily every object).
        /// </summary>
        public bool IsOn
        {
            get
            {
                return isOnInitialized ? isOn : initiallyOn;
            }

            set
            {
                Switch(value);
            }
        }
		

        /// <summary>
        /// Switch the group of object defined by the list on or off.
        /// </summary>
        /// <param name="on">Switch on (true) or off (false).</param>
        public void Switch(bool on)
        {
            if (isOnInitialized && (switchedObjects == null || isOn == on))
            {
                return;
            }
            if (!isOnInitialized)
            {
                SaveObjectStates();
            }
            isOn = on;
            if (on)
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

