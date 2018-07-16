// Code derived from Microsoft HoloToolkit
// (https://github.com/Microsoft/MixedRealityToolkit-Unity)

using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Keep a GameObject oriented towards the user.
    /// </summary>
    public class BillboardController : MonoBehaviour
    {
        /// <summary>
        /// Affect only the heading of the object.
        /// </summary>
        [Tooltip("Affect only the heading of the object")]
        public bool onlyHeading = false;

        /// <summary>
        /// Reference target for orientation (main camera if not set).
        /// </summary>
        [Tooltip("Reference target for orientation (main camera if not set)")]
        public Transform TargetTransform;

        private void OnEnable()
        {
            if (TargetTransform == null)
            {
                TargetTransform = Camera.main.transform;
            }

            Update();
        }

        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void Update()
        {
            if (TargetTransform == null)
            {
                return;
            }

            // Get a Vector that points from the target to the main camera.
            Vector3 directionToTarget = TargetTransform.position - transform.position;

            // Adjust for the pivot axis.
            if (onlyHeading)
            {
                directionToTarget.y = 0.0f;
            }

            // If we are right next to the camera the rotation is undefined. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return;
            }

            // Calculate and apply the rotation required to reorient the object
            transform.rotation = Quaternion.LookRotation(-directionToTarget);
        }
    }
}