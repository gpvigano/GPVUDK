using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// Route Tap event to UI click
    /// </summary>
    [RequireComponent(typeof(GestureDetector))]
    [RequireComponent(typeof(ObjectSelector))]
    public class TapToClick : MonoBehaviour
    {
        private GestureDetector gestureDetector;

        private ObjectSelector objectSelector;

        private void Awake()
        {
            gestureDetector = GetComponent<GestureDetector>();
            objectSelector = GetComponent<ObjectSelector>();
        }

        private void OnEnable()
        {
            // Register to tap events
            gestureDetector.Tap += OnTap;
        }

        private void OnDisable()
        {
            // Unregister from tap events
            gestureDetector.Tap -= OnTap;
        }

        /// <summary>
        /// Route Select message to OnClick or OnValueChanged message
        /// </summary>
        private void OnTap()
        {
            GameObject focusedObject = objectSelector.Selection;
            if (focusedObject!=null)
            {
                UiUtil.Click(focusedObject);
            }
        }
    }
}
