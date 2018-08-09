namespace GPVUDK
{
    using UnityEngine;
    public class AxisRotation : MonoBehaviour
    {
        [Tooltip("Angular velocity in degrees per seconds")]
        public float degPerSec = 60.0f;

        [Tooltip("Angular velocity variation pulse period in cycles/s (0=disable)")]
        public float angularPulsePeriod = 0;

        [Tooltip("Angular velocity variation pulse amplitude in degrees/second (if Angular Pulse Period > 0)")]
        public float angularPulseAmplitude = 1f;

        [Tooltip("Rotation axis")]
        public Vector3 rotAxis = Vector3.up;

        // Use this for initialization
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