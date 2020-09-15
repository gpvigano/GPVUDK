namespace GPVUDK
{
    using UnityEngine;

    /// <summary>
    /// Frame by frame controller for rotation around an axis.
    /// </summary>
    public class AxisRotation : MonoBehaviour
    {
        /// <summary>
        /// Angular velocity in degrees per seconds.
        /// </summary>
        [Tooltip("Angular velocity in degrees per seconds")]
        public float degPerSec = 60.0f;

        /// <summary>
        /// Angular velocity variation pulse period in cycles/s (0=disable).
        /// </summary>
        [Tooltip("Angular velocity variation pulse period in cycles/s (0=disable)")]
        public float angularPulsePeriod = 0;

        /// <summary>
        /// Angular velocity variation pulse amplitude in degrees/second (if Angular Pulse Period > 0).
        /// </summary>
        [Tooltip("Angular velocity variation pulse amplitude in degrees/second (if Angular Pulse Period > 0)")]
        public float angularPulseAmplitude = 1f;

        /// <summary>
        /// Rotation axis.
        /// </summary>
        [Tooltip("Rotation axis")]
        public Vector3 rotAxis = Vector3.up;

		
        private void Start()
        {
            rotAxis.Normalize();
        }

		
        private void Update()
        {
            if (angularPulsePeriod > 0)
            {
                //                        __ amplitude
                //     .'.        .'.        
                //    .   .      .   .       
                //   .     .    .     .      
                // .'       '..'       '. __ 0
                //|           |  period  |
                //            t          t+period
                float angVelVariation = angularPulseAmplitude * (0.5f + 0.5f * Mathf.Sin(Time.time * (2f * Mathf.PI) / angularPulsePeriod - Mathf.PI));
                transform.Rotate(rotAxis, (degPerSec + angVelVariation) * Time.deltaTime);
            }
            else
            {
                transform.Rotate(rotAxis, degPerSec * Time.deltaTime);
            }
        }
    }
}