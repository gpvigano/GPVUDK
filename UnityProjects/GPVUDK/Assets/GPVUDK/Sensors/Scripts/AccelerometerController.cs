using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Orientation of an object based only on accelerometer data.
    /// Useful for low-end devices with only an accelerometer.
    /// </summary>
    /// <remarks>If the Look Direction is set to Up or Down
    /// it is possible to use this script with a camera to
    /// look around both with vertical and horizontal
    /// roteation, otherwise the horizontal rotation can be
    /// achieved tilting the head to the left or to the right
    /// if tiltToRotate>0.</remarks>
    public class AccelerometerController : InputOrientationController
    {
        public enum LookDir { Front, Up, Down };

        [Tooltip("Default look direction (a further rotation will be applied if not equal to Front)")]
        public LookDir lookDirection = LookDir.Front;

        [Tooltip("If this is >0 heading can be changed tilting to the left or to the right")]
        [Range(0.0f, 10f)]
        public float tiltToRotate = 0f;

        private float headingRotation = 0;

        public void SetTiltToRotate(float val)
        {
            tiltToRotate = val;
        }
        public void SetLookDirection(LookDir lookDir)
        {
            lookDirection = lookDir;
        }
        public void SetUpLookDirection()
        {
            lookDirection = LookDir.Up;
        }
        public void SetFrontLookDirection()
        {
            lookDirection = LookDir.Front;
        }
        public void SetDownLookDirection()
        {
            lookDirection = LookDir.Down;
        }

        void Update()
        {
            AccelerationRotate();
        }

        void AccelerationRotate()
        {
            // acceleration, gravity included
            Vector3 accelVec = AccelerationVectorToUnity(Input.acceleration);
            Vector3 gDir = Vector3.down;
            switch (lookDirection)
            {
                case LookDir.Front:
                    gDir = Vector3.down;
                    break;
                case LookDir.Up:
                    gDir = Vector3.back;
                    break;
                case LookDir.Down:
                    gDir = Vector3.forward;
                    break;
            }
            Quaternion combinedRotation = Quaternion.Euler(0, headingRotation, 0) * Quaternion.FromToRotation(accelVec, gDir);
            float threshold = 0.8f;
            //float xMax = 0.85f;
            //float xMin = -1.130f;
            float xNorm = lookDirection == LookDir.Front ? accelVec.x * 2f : accelVec.x;// < 0 ? ag.x / -xMin : ag.x / xMax;
            float amount = Mathf.Sign(xNorm) * (Mathf.Abs(xNorm) - threshold) * tiltToRotate;
            if (tiltToRotate >= 1f && Mathf.Abs(xNorm) > threshold)
            {
                switch (lookDirection)
                {
                    case LookDir.Front:
                        headingRotation += amount;
                        break;
                    case LookDir.Up:
                        headingRotation += amount;
                        break;
                    case LookDir.Down:
                        headingRotation -= amount;
                        break;
                }
            }

            ApplyRotation(combinedRotation);
        }
    }
}
