using System.Collections;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// General visualization utilities.
    /// </summary>
    public class MotionUtil
    {
        /// <summary>
        /// Move an object to the given position and (optionally) rotation.
        /// </summary>
        /// <param name="movedObject">Game object to be moved</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="targetRot">Target rotation (ignored if null)</param>
        public static void MoveObject(GameObject movedObject, Vector3 targetPos, Quaternion? targetRot = null)
        {
            if (movedObject)
            {
                movedObject.transform.position = targetPos;
                if (targetRot.HasValue)
                {
                    movedObject.transform.rotation = targetRot.Value;
                }
            }
        }


        /// <summary>
        /// Push the rigid body of a game object towards the given position and (optionally) rotation.
        /// </summary>
        /// <param name="movedObject">Game object to be moved (its rigid body will be affected)</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="targetRot">Target rotation (ignored if null)</param>
        /// <param name="maxG">Maximum G-force applied to the rigid body (0 means no limit)</param>
        /// <remarks>Setting a maximum applied G-force slows down the movements, but grants a better stability, mainly when synchronizing objects across the network.</remarks>
        public static void MoveRigidbody(GameObject movedObject, Vector3 targetPos, Quaternion? targetRot = null, float maxG = 20f)
        {
            if (movedObject == null)
            {
                return;
            }
            Rigidbody rigidBody = movedObject.GetComponent<Rigidbody>();
            if (rigidBody)
            {
                float t = Mathf.Max(Time.smoothDeltaTime, Time.deltaTime);
                Vector3 shift = targetPos - rigidBody.position;
                Vector3 velocity = shift / t;
                Vector3 acceleration = velocity;
                rigidBody.velocity = Vector3.zero;
                Vector3 force = rigidBody.mass * acceleration;
                Vector3 gForce = rigidBody.mass * Physics.gravity;// Vector3.down * 9.81f;

                // limit accelerations for a better stability
                if (maxG > 0 && force.magnitude > gForce.magnitude * maxG)
                {
                    force = force.normalized * gForce.magnitude * maxG;
                }
                // compensate gravity
                if (rigidBody.useGravity)
                {
                    force -= gForce;
                }
                rigidBody.AddForce(force);

                if (targetRot != null)
                {
                    //float degPerSec = 60f;
                    //rigidBody.rotation = Quaternion.RotateTowards(movedObject.transform.rotation, targetRot.Value, degPerSec/t);
                    rigidBody.rotation = targetRot.Value;
                }
            }
            else
            {
                Debug.LogWarning("MoveRigidbody() - missing rigidbody in object " + movedObject.name);
            }
        }
    }
}
