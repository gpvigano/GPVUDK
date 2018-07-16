using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class MenuToggleButton : MonoBehaviour
    {
        public GameObject menuObject;
        public GameObject menuButton;
        public bool startOpen = true;

        public void ToggleMenu()
        {
            SetMenu( !menuObject.activeSelf);
        }

        public void SetMenu(bool on)
        {
            menuObject.SetActive(on);
            if(menuButton!=null)
            {
                menuButton.SetActive(!on);
            }
        }

        void Start()
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
