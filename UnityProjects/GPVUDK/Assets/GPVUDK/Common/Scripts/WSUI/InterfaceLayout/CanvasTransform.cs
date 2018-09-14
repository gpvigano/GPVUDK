using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    [Serializable]
    public class CanvasTransform
    {
        public Transform scaleTransform;
        public Transform yPositionTransform;
        public Transform xRotationTransform;
        public float scale = 0.0005f;
        public float yPosition = 0;
        public float xRotation = 0;

        public void Apply()
        {
            if (xRotationTransform != null)
            {
                Vector3 angles = xRotationTransform.localEulerAngles;
                angles.x = xRotation;
                xRotationTransform.localEulerAngles = angles;
            }
            if (yPositionTransform != null)
            {
                Vector3 pos = yPositionTransform.localPosition;
                pos.y = yPosition;
                yPositionTransform.localPosition = pos;
            }
            if (scaleTransform != null)
            {
                scaleTransform.localScale = Vector3.one * scale;
            }
        }

        public void Store()
        {
            if (xRotationTransform != null)
            {
                xRotation = xRotationTransform.localEulerAngles.x;
                xRotation = Mathf.Repeat(xRotation + 180f, 360f) - 180f;
            }
            if (scaleTransform != null)
            {
                scale = scaleTransform.localScale.x;
            }
            if (yPositionTransform != null)
            {
                yPosition = yPositionTransform.localPosition.y;
            }
        }

        public void Validate()
        {
            if(xRotation>180f)
            {
                xRotation -= 360f;
            }
        }
    }
}