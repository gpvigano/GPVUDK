using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Base abstract class 
    /// </summary>
    [RequireComponent(typeof(GestureDetector))]
    [RequireComponent(typeof(ObjectSelector))]
    public class PointerController : MonoBehaviour
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
            // Register to gesture events
            gestureDetector.Activate += OnActivate;
            gestureDetector.Deactivate += OnDeactivate;
            gestureDetector.Tap += OnTap;
        }

        private void OnDisable()
        {
            // Unregister from gesture events
            gestureDetector.Activate -= OnActivate;
            gestureDetector.Deactivate -= OnDeactivate;
            gestureDetector.Tap -= OnTap;
        }

        /// <summary>
        /// Route Select message to OnClick or OnValueChanged message
        /// </summary>
        private void OnActivate()
        {
            GameObject focusedObject = objectSelector.Selection;
            if (focusedObject!=null)
            {
                UiUtil.PointerDown(focusedObject);
            }
        }

        /// <summary>
        /// Route Select message to OnClick or OnValueChanged message
        /// </summary>
        private void OnDeactivate()
        {
            GameObject focusedObject = objectSelector.Selection;
            if (focusedObject!=null)
            {
                UiUtil.PointerUp(focusedObject);
            }
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
