using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class CompassGravityController : InputOrientationController
    {
        protected override void Start()
        {
            base.Start();
            Input.compass.enabled = true;
            Input.gyro.enabled = true;

            // To let Input.compass.trueHeading property contain a valid value,
            // enable location updates by calling:
            Input.location.Start();
            // TODO: Input.compass.trueHeading does not work

        }

        void Update()
        {
            CompassGravityRotate();
        }

        void CompassGravityRotate()
        {
            // The Gyroscope is right-handed.  Unity is left handed.
            // Make the necessary change.
            Vector3 g = AccelerationVectorToUnity(Input.gyro.gravity);
            Vector3 gDir = Vector3.down;
            Quaternion rotation = Quaternion.FromToRotation(g, gDir);
            Quaternion combinedRotation = Quaternion.Euler(0, Input.compass.trueHeading, 0) * rotation;
            ApplyRotation(combinedRotation);
        }
    }
}
