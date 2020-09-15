using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// Control of the interaction state of multiple UI widgets.
    /// </summary>
    public class MultiWidgetSwitch : MonoBehaviour
    {
        /// <summary>
        /// All the widgets in this list can be set interactable or not at once.
        /// </summary>
        [Tooltip("All the widgets in this list can be set interactable or not at once")]
        public Selectable[] widgetList;

        /// <summary>
        /// Set all the widgets in the above list interactable or not on start.
        /// </summary>
        [Tooltip("Set all the widgets in the above list interactable or not on start")]
        [SerializeField]
        public bool setOnStart = false;


        /// <summary>
        /// Set all the widgets in the above list interactable on start.
        /// </summary>
        [Tooltip("Set all the widgets in the above list interactable on start")]
        [SerializeField]
        public bool initiallyInteractable = false;


		/// <summary>
        /// Set all the controlled widgets to interactable or not.
        /// </summary>
        /// <param name="interactable">true=interactable, false=not interactable</param>
        public void SetInteractable(bool interactable)
        {
            foreach(Selectable sel in widgetList)
            {
                sel.interactable = interactable;
            }
        }

		
        private void Start()
        {
        }
		

        private void Update()
        {
        }
    }
}