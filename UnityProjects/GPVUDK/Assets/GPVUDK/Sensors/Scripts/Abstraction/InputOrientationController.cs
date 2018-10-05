using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Base class for object orientation based on device input.
    /// </summary>
    public class InputOrientationController : MonoBehaviour
    {
        [Tooltip("Object to be rotated")]
        public Transform controlledObject;

        [Tooltip("Motion filtering (higher value, smoother motion)")]
        [Range(0.0f, 0.999f)]
        public float smoothness = 0.5f;

        [Tooltip("Prevent screen dimming and switch off")]
        [SerializeField]
        protected bool disableScreenPowerSaving = false;

        protected Quaternion smoothRotation = Quaternion.identity;
        protected bool smoothingStarted = false;

        /// <summary>
        /// UI slider value change event handler
        /// </summary>
        /// <param name="val">Slider value</param>
        public void SetSmoothness(float val)
        {
            smoothness = val;
        }

        protected virtual void OnValidate()
        {

            if (controlledObject == null)
            {
                controlledObject = transform;
            }
        }

        protected virtual void Start()
        {
            if (disableScreenPowerSaving)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }

        /// <summary>
        /// Smooth the given rotation according to the smoothness field value.
        /// </summary>
        /// <param name="rawRotation">Rotation to be smoothed</param>
        /// <returns>Smoothed rotation.</returns>
        protected virtual Quaternion ComputeSmoothRotation(Quaternion rawRotation)
        {
            if (!smoothingStarted)
            {
                smoothRotation = rawRotation;
                smoothingStarted = true;
            }

            smoothRotation = Quaternion.Slerp(rawRotation, smoothRotation, smoothness);
            return smoothRotation;
        }

        protected virtual void ApplyRotation(Quaternion combinedRotation)
        {
            controlledObject.rotation = ComputeSmoothRotation(combinedRotation);
        }

        /// <summary>
        /// Convert a rotation from device right-handed coordinate system to Unity left handed coordinate system.
        /// </summary>
        /// <param name="q">Rotation in device coordinate system</param>
        /// <returns>Rotation  in Unity coordinate system</returns>
        public static Quaternion GyroToUnity(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        /// <summary>
        /// Convert a vector from device right-handed coordinate system to Unity left handed coordinate system.
        /// </summary>
        /// <param name="accelerationVector">Acceleration vector in device coordinate system</param>
        /// <returns>Acceleration vector in Unity coordinate system</returns>
        public static Vector3 AccelerationVectorToUnity(Vector3 accelerationVector)
        {
            // The Gyroscope is right-handed.  Unity is left handed.
            // Make the necessary changes.
            Vector3 ag = Vector3.zero;
            ag.x = accelerationVector.x;
            ag.y = accelerationVector.y;
            ag.z = -accelerationVector.z;
            return ag;
        }
    }
}
