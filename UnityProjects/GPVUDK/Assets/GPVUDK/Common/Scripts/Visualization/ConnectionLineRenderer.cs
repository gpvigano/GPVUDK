using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Draw a connection line betweeen two positions using a LineRenderer component.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionLineRenderer : MonoBehaviour
    {
        [Tooltip("Transform for line start position. If not provided the position of this game object transform is used.")]
        public Transform drawLineFrom;
        [Tooltip("Transform for the line end position.")]
        public Transform drawLineTo;
        [Tooltip("Offset for the line end position.")]
        public Vector3 endOffset = Vector3.zero;

        private LineRenderer lineRenderer;

        private void OnValidate()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if(lineRenderer!=null)
            {
                if(drawLineFrom==null)
                {
                    drawLineFrom = transform;
                }
                DrawLine();
            }
           
        }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if(drawLineFrom==null)
            {
                drawLineFrom = transform;
            }
            lineRenderer.useWorldSpace = true;
        }

        private void Update()
        {
            DrawLine();
        }

        private void DrawLine()
        {
            if (drawLineTo!=null)
            {
                lineRenderer.SetPosition(0, drawLineFrom.position);
                lineRenderer.SetPosition(1, drawLineTo.position+ endOffset);
            }
        }
    }
}
