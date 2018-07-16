using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class ObjectInfo : MonoBehaviour
    {

        public string identifier;
        [Multiline]
        public string description;
        public AudioClip audioClip;
        public bool autoRead = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
