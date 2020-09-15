

using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Keep a GameObject oriented towards a given target Transform (e.g. main camera).
    /// </summary>
    public class BillboardController : MonoBehaviour
    {
        /// <summary>
        /// Affect only the heading of the object.
        /// </summary>
        [Tooltip("Affect only the heading of the object")]
        public bool onlyHeading = false;

        /// <summary>
        /// Orient this object back facing the target transform.
        /// </summary>
        [Tooltip("Orient this object back facing the target transform")]
        public bool backFacing = false;

        /// <summary>
        /// Reference target for orientation (main camera if not set).
        /// </summary>
        [Tooltip("Reference target for orientation (main camera if not set)")]
        public Transform targetTransform;

		
        /// <summary>
        /// Set the reference target for orientation to main camera if not yet set and update.
        /// </summary>
        private void OnEnable()
        {
            if (targetTransform == null)
            {
                targetTransform = Camera.main.transform;
            }
            Update();
        }

		
        /// <summary>
        /// Keeps the object facing (or back facing) the given Transform.
        /// </summary>
        private void Update()
        {
            if (targetTransform == null)
            {
                return;
            }

            // Get a Vector that points from the this object to the given Transform.
            Vector3 directionToTarget = targetTransform.position - transform.position;

            // Project the vector on XZ plane if the onlyHeading flag is set.
            if (onlyHeading)
            {
                directionToTarget.y = 0.0f;
            }

            // Skip calculations if too close to the target to avoid computation errors. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return;
            }

            // Calculate and apply the rotation required to reorient the object
            transform.rotation = Quaternion.LookRotation(backFacing ? directionToTarget : -directionToTarget);
        }
    }
}