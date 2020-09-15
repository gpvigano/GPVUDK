using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Draw a connection line betweeen two positions using a LineRenderer component.
    /// </summary>
    public class MultiConnectionLineRenderer : MonoBehaviour
    {
        /// <summary>
        /// Width of the lines in metres.
        /// </summary>
        [Tooltip("Width of the lines in metres.")]
        public float lineWidth = 0.002f;
		
        /// <summary>
        /// Material used to render the lines.
        /// </summary>
        [Tooltip("Material used to render the lines.")]
        public Material lineMaterial;

        /// <summary>
        /// Transform for line start position. If not provided the position of this game object transform is used.
        /// </summary>
        [Tooltip("Transform for line start position. If not provided the position of this game object transform is used.")]
        public Transform drawLineFrom;
		
        /// <summary>
        /// Offset for the line start position.
        /// </summary>
        [Tooltip("Offset for the line start position.")]
        public Vector3 startOffset = Vector3.zero;
		
        [SerializeField]
        [Tooltip("Transform for the line end position.")]
        private Transform[] drawLineTo;

        private GameObject[] lineRendererObjects;

		/// <summary>
        /// Transform for the line end position.
        /// </summary>
        public Transform[] DrawLineTo
        {
            get
            {
                return drawLineTo;
            }

            set
            {
                DestroyConnectionLines();
                drawLineTo = value;
                BuildConnectionLines();
            }
        }

		
        private void BuildConnectionLines()
        {
            DestroyConnectionLines();
            if (drawLineTo.Length > 0)
            {
                lineRendererObjects = new GameObject[drawLineTo.Length];
                for (int i = 0; i < drawLineTo.Length; i++)
                {
                    if (drawLineTo[i] != null)
                    {
                        lineRendererObjects[i] = new GameObject(name + "_LINE" + i.ToString());
                        lineRendererObjects[i].transform.SetParent(transform, false);
                        LineRenderer lineRenderer = lineRendererObjects[i].AddComponent<LineRenderer>();
                        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
                        lineRenderer.material = lineMaterial;
                        lineRenderer.useWorldSpace = true;
                        ConnectionLineRenderer connectionLineRenderer = lineRendererObjects[i].AddComponent<ConnectionLineRenderer>();
                        connectionLineRenderer.drawLineFrom = drawLineTo[i];
                        connectionLineRenderer.drawLineTo = drawLineFrom;
                        connectionLineRenderer.endOffset = startOffset;
                    }
                }
            }
        }

		
        private void DestroyConnectionLines()
        {
            if (lineRendererObjects != null)
            {
                foreach (GameObject obj in lineRendererObjects)
                {
                    Destroy(obj);
                }
                lineRendererObjects = null;
            }
        }

		
        private void Awake()
        {
            if (drawLineFrom == null)
            {
                drawLineFrom = transform;
            }
        }

		
        private void Start()
        {
            BuildConnectionLines();
        }
		

        private void OnDestroy()
        {
            DestroyConnectionLines();
        }
    }
}
