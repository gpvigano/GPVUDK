using System;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Place this script on the cursor main game object, add cursor representations as children.
    /// </summary>
    public class GazeCursorController : MonoBehaviour
    {
        [Tooltip("Distance used to place the cursor when nothing is gazed.")]
        /// <summary>
        /// Distance used to place the cursor when nothing is gazed.
        /// </summary>
        public float defaultDistance = 2f;
        [Tooltip("Offset along the gazed object normal.")]
        /// <summary>
        /// Offset along the gazed object normal.
        /// </summary>
        public float surfaceOffset = 0.02f;
        [Tooltip("Orient the cursor along the gazed surface normal.")]
        /// <summary>
        /// Orient the cursor along the gazed surface normal.
        /// </summary>
        public bool followSurfaceNormal = false;
        [Tooltip("Cursor representation when nothing is gazed.")]
        /// <summary>
        /// Cursor representation when nothing is gazed.
        /// </summary>
        public GameObject defaultCursor = null;
        [Tooltip("Cursor representation when something is gazed.")]
        /// <summary>
        /// Cursor representation when something is gazed.
        /// </summary>
        public GameObject objectHitCursor = null;
        [Tooltip("Cursor representation when interacting with something.")]
        /// <summary>
        /// Cursor representation when interacting with something.
        /// </summary>
        public GameObject interactCursor = null;

        [Header("Interaction managers")]
        [Tooltip("Gaze Selector script (if not set it will be searched everywhere).")]
        [SerializeField]
        private GazeSelector gazeSelector;
        [Tooltip("Gesture Detector script (if not set it will be searched everywhere).")]
        [SerializeField]
        private GestureDetector gestureDetector = null;
        [Tooltip("UI World Pointer script (if not set it will be got from this object).")]
        [SerializeField]
        private UIWorldPointer uiWorldPointer;

        private GameObject hitObject = null;
        private bool isInteracting = false;
        private float currentDistance;
        private Draggable activeDraggable = null;

        /// <summary>
        /// True if interacting with an object (e.g. dragging an object)
        /// </summary>
        public bool IsInteracting
        {
            get
            {
                return activeDraggable != null && isInteracting;
            }
            set
            {
                if (isInteracting != value)
                {
                    isInteracting = value;
                    if (interactCursor != null)
                    {
                        interactCursor.SetActive(isInteracting);
                    }
                }
            }
        }

        protected virtual void SwitchCursorObject(GameObject obj)
        {
            if (objectHitCursor != null)
            {
                objectHitCursor.SetActive(obj == objectHitCursor);
            }
            if (defaultCursor != null)
            {
                defaultCursor.SetActive(obj == defaultCursor);
            }
            if (interactCursor != null)
            {
                interactCursor.SetActive(obj == isInteracting);
            }
        }

        protected virtual void SetHitObject(GameObject obj)
        {
            if (hitObject == obj || IsInteracting)
            {
                return;
            }
            SwitchCursorObject((obj != null) ? objectHitCursor : defaultCursor);
        }

        protected virtual void SetInteracting(bool on)
        {
            IsInteracting = on;
            SwitchCursorObject(on ? interactCursor : objectHitCursor);
            if (uiWorldPointer != null)
            {
                uiWorldPointer.enabled = !on;
            }
        }

        protected virtual void Awake()
        {
            if (gazeSelector == null)
            {
                gazeSelector = FindObjectOfType<GazeSelector>();
            }
            if (gestureDetector == null)
            {
                gestureDetector = FindObjectOfType<GestureDetector>();
            }
            uiWorldPointer = GetComponent<UIWorldPointer>();
        }

        protected virtual void OnEnable()
        {
            if (gazeSelector != null)
            {
                // Register to gaze events
                gazeSelector.SelectionChanged += OnFocusedObjectChanged;
            }
            if (gestureDetector != null)
            {
                // Register to gesture events
                gestureDetector.Activate += OnStartInteracting;
                gestureDetector.Deactivate += OnStopInteracting;
            }
            if (gazeSelector != null)
            {
                SetHitObject(gazeSelector.Selection);
            }
        }

        protected virtual void OnDisable()
        {
            if (gazeSelector != null)
            {
                // Unregister to gaze events
                gazeSelector.SelectionChanged -= OnFocusedObjectChanged;
            }
            if (gestureDetector != null)
            {
                // Unregister to gesture events
                gestureDetector.Activate -= OnStartInteracting;
                gestureDetector.Deactivate -= OnStopInteracting;
            }
        }

        protected virtual void OnFocusedObjectChanged(GameObject previousObject, GameObject newObject)
        {
            if (!IsInteracting)
            {
                SetHitObject(newObject);
            }
        }

        protected virtual void OnStartInteracting()
        {
            if (!IsInteracting)
            {
                Vector3 gazeOrigin = gazeSelector.GazeOrigin;
                Vector3 gazeDirection = gazeSelector.GazeDirection;

                hitObject = gazeSelector.Selection;
                if (hitObject != null)
                {
                    activeDraggable = hitObject.GetComponentInParent<Draggable>();
                    if (activeDraggable != null)
                    {
                        transform.position = gazeOrigin + gazeDirection * currentDistance;
                        transform.forward = gazeDirection;
                        activeDraggable.StartDragging(transform);
                        SetInteracting(true);
                    }
                }
            }
        }

        protected virtual void OnStopInteracting()
        {
            if (IsInteracting)
            {
                activeDraggable.StopDragging();
                SetInteracting(false);
            }
        }

        private void Update()
        {
            Vector3 gazeOrigin = gazeSelector.GazeOrigin;
            Vector3 gazeDirection = gazeSelector.GazeDirection;

            if (IsInteracting)
            {
                transform.position = gazeOrigin + gazeDirection * currentDistance;
                transform.forward = gazeDirection;
            }
            else if (gazeSelector.SomethingSelected)
            {
                transform.position = gazeSelector.HitPoint - gazeDirection * surfaceOffset;
                transform.forward = followSurfaceNormal ? gazeSelector.HitNormal : gazeDirection;
            }
            else
            {
                transform.position = gazeOrigin + gazeDirection * defaultDistance;
                transform.forward = gazeDirection;
            }
            currentDistance = Vector3.Distance(transform.position, gazeOrigin);
        }
    }
}
