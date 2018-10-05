using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Simple class used to store information about a game object.
    /// Used by SwitchObject script.
    /// </summary>
    public class ObjectInfo : MonoBehaviour
    {
        [Tooltip("Identifier/name")]
        public string identifier;
        [Multiline]
        [Tooltip("Textual description")]
        public string description;
        [Tooltip("Audio description/Related sound clip")]
        public AudioClip audioClip;
        [Tooltip("Automatically say the audio description/play the sound")]
        public bool autoSay = false;
    }
}
