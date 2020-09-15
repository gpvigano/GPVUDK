using System;
using UnityEngine;
using System.Collections.Generic;
using GPVUDK;

namespace GPVUDK
{
    /// <summary>
    /// Highlight a given game object.
    /// </summary>
    public class ObjectHighlighter : MonoBehaviour
    {
        /// <summary>
        /// Color used for the highlighting material.
        /// </summary>
        [Tooltip("Color used for the highlighting material")]
        public Color highlightColor = Color.cyan;

        protected GameObject currentObject = null;
        protected Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
        protected Dictionary<Renderer, Material> selectionMaterials = new Dictionary<Renderer, Material>();
		

		/// <summary>
        /// An object is highlighted.
        /// </summary>
        public bool Highlighted
        {
            get { return currentObject != null; }
        }

		
		/// <summary>
        /// Event triggered when an object is highlighted
        /// </summary>
        public event Action<GameObject> ObjectHighlighted;

        /// <summary>
        /// Event triggered when an object is no more highlighted
        /// </summary>
        public event Action<GameObject> ObjectUnhighlighted;


		/// <summary>
        /// Stop highlighting the currentlty highlighted object.
        /// </summary>
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

		
        /// <summary>
        /// Highlight the given game object.
        /// </summary>
        /// <param name="obj"></param>
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