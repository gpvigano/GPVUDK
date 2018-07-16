using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Detect gaze on objects and trigger an Ungazed event for previous object and a Gazed event for the new one
    /// </summary>
    public class GazeSelector : ObjectSelector
    {
        [Tooltip("Transform used to control the gaze (None = main camera)")]
        [SerializeField]
        private Transform gazeTransform;

        /// <summary>
        /// Origin point of the gaze.
        /// </summary>
        public Vector3 GazeOrigin { get; protected set; }
        /// <summary>
        /// Direction of the gaze (forward).
        /// </summary>
        public Vector3 GazeDirection { get; protected set; }
        /// <summary>
        /// Last point hit by the gaze.
        /// </summary>
        public Vector3 HitPoint { get; protected set; }
        /// <summary>
        /// Normal of the surface in the last point hit by the gaze.
        /// </summary>
        public Vector3 HitNormal { get; protected set; }

        protected virtual void Awake()
        {
            if(gazeTransform==null)
            {
                gazeTransform = Camera.main.transform;
            }
        }

        protected virtual void Update()
        {
            // Figure out which hologram is focused this frame.
            GameObject oldHitObject = Selection;

            // Do a raycast into the world based on the user's
            // head position and orientation.
            GazeOrigin = gazeTransform.position;
            GazeDirection = gazeTransform.forward;
            RaycastHit hitInfo;
            SomethingSelected = Physics.Raycast(GazeOrigin, GazeDirection, out hitInfo);
            if (SomethingSelected)
            {
                HitPoint = hitInfo.point;
                HitNormal = hitInfo.normal;
                // If the raycast hit a hologram, use that as the focused object.
                OnSelectionChanged(oldHitObject, hitInfo.collider.gameObject);
            }
            else
            {
                // If the raycast did not hit a hologram, clear the focused object.
                OnSelectionChanged(oldHitObject, null);
            }
        }
    }
}

