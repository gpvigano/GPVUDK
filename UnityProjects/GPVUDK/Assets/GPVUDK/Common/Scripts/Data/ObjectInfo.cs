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
        /// <summary>
        /// Identifier/name.
        /// </summary>
        [Tooltip("Identifier/name")]
        public string identifier;
		
        [Multiline]
        /// <summary>
        /// Textual description.
        /// </summary>
        [Tooltip("Textual description")]
        public string description;
		
        /// <summary>
        /// Related sound clip.
        /// </summary>
        [Tooltip("Related sound clip")]
        public AudioClip audioClip;
		
        /// <summary>
        /// Automatically play the sound.
        /// </summary>
        [Tooltip("Automatically play the sound")]
        public bool autoPlay = false;
    }
}
