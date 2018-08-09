using System;
using UnityEngine;
using System.Collections.Generic;
using GPVUDK;

namespace GPVUDK.Tests
{
    public class ObjectHighlighter : MonoBehaviour
    {
        public Color highlightColor = Color.cyan;
        public bool Highlighted
        {
            get { return currentObject != null; }
        }
        public event Action<GameObject> ObjectHighlighted;
        public event Action<GameObject> ObjectUnhighlighted;

        protected GameObject currentObject = null;
        protected Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
        protected Dictionary<Renderer, Material> selectionMaterials = new Dictionary<Renderer, Material>();

        public virtual void Unhighlight()
        {
            if (currentObject != null)
            {
                // unhighlight previous object
                VisUtil.ReplaceMaterials(currentObject, originalMaterials);
                originalMaterials.Clear();
                selectionMaterials.Clear();
                GameObject obj = currentObject;
                currentObject = null;
                if (ObjectUnhighlighted != null)
                {
                    ObjectUnhighlighted(obj);
                }
            }
        }

        public virtual void Highlight(GameObject obj)
        {
            if (obj != null && obj!=currentObject)
            {
                currentObject = obj;
                Highlight(currentObject, highlightColor);
                if (ObjectHighlighted != null)
                {
                    ObjectHighlighted(obj);
                }
            }
        }

        protected virtual void OnDisable()
        {
            Unhighlight();
            currentObject = null;
        }

        protected virtual void Highlight(GameObject obj, Color color)
        {
            if (obj != null)
            {
                // highlight new object
                VisUtil.ChangeColor(obj, color, ref originalMaterials, ref selectionMaterials);
            }
        }
    }
}