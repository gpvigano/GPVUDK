using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Script to toggle a UI panel using a button.
    /// This should be attached to a game object containing both
    /// the menu and the button game objects as children.
    /// </summary>
    public class MenuToggleButton : MonoBehaviour
    {
        /// <summary>
        /// Game object containing the menu.
        /// </summary>
        public GameObject menuObject;
        /// <summary>
        /// Game object containing the button.
        /// </summary>
        public GameObject menuButton;
        /// <summary>
        /// Start with the menu open (start with the button if false)
        /// </summary>
        public bool startOpen = true;

        /// <summary>
        /// Toggle the menu. This can be called by the menu opening and closing buttons.
        /// </summary>
        public void ToggleMenu()
        {
            SetMenu( !menuObject.activeSelf);
        }

        /// <summary>
        /// Switch the menu open or closed. This can be called by the menu opening and closing buttons.
        /// </summary>
        public void SetMenu(bool on)
        {
            menuObject.SetActive(on);
            if(menuButton!=null)
            {
                menuButton.SetActive(!on);
            }
        }

        private void Start()
        {
            if(menuObject!=null)
            {
                SetMenu(startOpen);
            }
            else
            {
                Debug.LogError(GetType().Name + " component requires Menu Object set.");
            }
        }
    }
}
