using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class ScanningAnimationController : MonoBehaviour
    {
        public Material scanningMaterial;
        public AudioSource scanningAudio;
        [Range(0, 10f)]
        public float scanningRadius = 3f;
        public float scanningDuration = 3f;

        private float startTime = 0;

        void Start()
        {
            startTime = Time.realtimeSinceStartup;
            if (scanningAudio != null)
            {
                if (scanningDuration < scanningAudio.clip.length)
                {
                    scanningDuration = scanningAudio.clip.length;
                }
                scanningAudio.playOnAwake = false;
                scanningAudio.loop = false;
                scanningAudio.Stop();
                scanningAudio.Play();
            }
        }

        void Update()
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            float t = elapsedTime / scanningDuration;
            float r = Mathf.Lerp(0, scanningRadius, t);
            if (scanningMaterial != null && scanningMaterial.HasProperty("_Radius"))
            {
                scanningMaterial.SetFloat("_Radius", r);
            }

            if (scanningAudio != null && !scanningAudio.isPlaying && elapsedTime > scanningDuration)
            {
                startTime = Time.realtimeSinceStartup;
                scanningAudio.Play();
                if (scanningMaterial != null && scanningMaterial.HasProperty("_Centre"))
                {
                    scanningMaterial.SetVector("_Centre", transform.position);
                }
            }
        }
    }
}
