using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public class TransformReset : MonoBehaviour
    {
        [Tooltip("Transforms to be reset")]
        public Transform[] toReset;
        [Tooltip("Reference transform (optional): other transforms will be reset relative to its position/rotation")]
        public Transform referenceTransform=null;
        public bool resetPosition = true;
        public bool resetRotation = true;
        public bool resetScale = false;
        [Tooltip("If Reference Transform is set use its heading to reorientate the target transforms")]
        public bool useReferenceHeading = false;

        private struct TransformData
        {
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scale;
        }

        private Dictionary<Transform, TransformData> initTransform = new Dictionary<Transform, TransformData>();
        private Vector3 initReferencePos;
        private float initReferenceHeading;

        public void ResetTransforms()
        {
            Vector3 posOffset = Vector3.zero;
            float rotOffset = 0;
            if(referenceTransform!=null)
            {
                posOffset = referenceTransform.position - initReferencePos;
                if (useReferenceHeading)
                {
                    rotOffset = referenceTransform.rotation.eulerAngles.y - initReferenceHeading;
                }
            }
            foreach(Transform tr in toReset)
            {
                TransformData data = initTransform[tr];
                if (resetPosition)
                {
                    tr.localPosition = data.pos+posOffset;
                }
                if (resetRotation)
                {
                    tr.localRotation = data.rot;
                    if (useReferenceHeading)
                    {
                        tr.RotateAround(referenceTransform.position,Vector3.up, rotOffset);
                    }                          
                }
                if (resetScale)
                {
                    tr.localScale = data.scale;
                }
            }
        }

        private void Awake()
        {
            foreach(Transform tr in toReset)
            {
                TransformData data;
                data.pos = tr.localPosition;
                data.rot = tr.localRotation;
                data.scale = tr.localScale;
                initTransform[tr] = data;
            }
            if(referenceTransform!=null)
            {
                initReferencePos = referenceTransform.position;
                if (useReferenceHeading)
                {
                    initReferenceHeading = referenceTransform.rotation.eulerAngles.y;
                }
            }
        }
    }
}
