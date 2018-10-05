using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Highlight objects and optionally hide others, even automatically on focus.
    /// </summary>
    /// <remarks>If highlight objects are shown even if not highlighted
    /// they should have an Animator to make highlighting work.</remarks>
    public class HighlightTrigger : MonoBehaviour
    {
        [Tooltip("Highlight only one object")]
        public bool onlyOne = true;
        [Tooltip("Automatically highlight an object in the list if focused")]
        public bool autoHighlightOnFocus = false;
        [Tooltip("Show the highlight (with no animation) even when nothing is highlighted")]
        public bool showEvenIfNotHighlighted = false;
        [Tooltip("Hide the the objects in the below list")]
        public bool hideObjects = false;
        [Tooltip("Drop here the objects to highlight")]
        public GameObject[] objectsToHighlight;
        [Tooltip("Drop here the objects to hide on highlight")]
        public GameObject[] objectsToHide;

        public GameObject LastHighlightedObject { get; private set; }

        private ObjectSelector objectSelector;
        private GestureDetector tapDetector;
        private Dictionary<GameObject, Renderer[]> rendererToHighlight = new Dictionary<GameObject, Renderer[]>();
        private AudioSource audioSource;

        public int NumHighlightObjects
        {
            get
            {
                return objectsToHighlight != null ? objectsToHighlight.Length : 0;
            }
        }

        public GameObject GetPreviousHighlightObject(GameObject obj)
        {
            GameObject prevObj = null;
            foreach (GameObject hlObj in objectsToHighlight)
            {
                if (hlObj == obj)
                {
                    return prevObj;
                }
                prevObj = hlObj;
            }
            return null;
        }

        public GameObject GetNextHighlightObject(GameObject obj)
        {
            bool found = false;
            foreach (GameObject hlObj in objectsToHighlight)
            {
                if (found)
                {
                    return hlObj;
                }
                found = (hlObj == obj);
            }
            return null;
        }

        public event Action<GameObject> FocusOnObject;
        public event Action<GameObject> ObjectSelected;

        /// <summary>
        /// Highlight the next object in the list.
        /// </summary>
        /// <returns>Returns true if the next object is available, false if reached the end of the list.</returns>
        public bool HighlightNext()
        {
            bool nextFound = false;
            foreach (GameObject obj in objectsToHighlight)
            {
                if (LastHighlightedObject == null || nextFound)
                {
                    Highlight(obj);
                    return true;
                }
                nextFound = (LastHighlightedObject == obj);
            }
            return false;
        }


        /// <summary>
        /// Highlight the previous object in the list.
        /// </summary>
        /// <returns>Returns true if the previous object is available, false if reached the beginning of the list.</returns>
        public bool HighlightPrevious()
        {
            bool prevFound = false;
            for (int i = objectsToHighlight.Length - 1; i >= 0; i--)
            {
                GameObject obj = objectsToHighlight[i];
                if (LastHighlightedObject == null || prevFound)
                {
                    Highlight(obj);
                    return true;
                }
                prevFound = (LastHighlightedObject == obj);
            }
            return false;
        }

        public virtual void SwitchHighlight(GameObject obj, bool on, bool affectRendering, bool affectAnimator)
        {
            if (!rendererToHighlight.ContainsKey(obj))
            {
                return;
            }
            Animator anim = obj.GetComponentInChildren<Animator>();
            if (affectAnimator && anim != null)
            {
                anim.Rebind();
                anim.enabled = on;
                if (anim.gameObject.activeInHierarchy)
                {
                    anim.Update(Time.deltaTime);
                }
            }
            if (affectRendering)
            {
                foreach (Renderer mr in rendererToHighlight[obj])
                {
                    mr.enabled = on;
                }
            }

        }

        /// <summary>
        /// Highlight the given object.
        /// </summary>
        /// <param name="newObject">object to highlight</param>
        public void Highlight(GameObject newObject)
        {
            if (newObject == null)
            {
                foreach (GameObject obj in objectsToHighlight)
                {
                    SwitchHighlight(obj, showEvenIfNotHighlighted, true, false);
                    SwitchHighlight(obj, false, false, true);
                }
            }
            else
            {
                foreach (GameObject obj in objectsToHighlight)
                {
                    if (obj == newObject)
                    {
                        SwitchHighlight(obj, true, true, true);
                    }
                    else
                    {
                        SwitchHighlight(obj, false, onlyOne, true);
                    }
                }
            }
            foreach (GameObject obj in objectsToHide)
            {
                obj.SetActive(!hideObjects || newObject == null);
            }
            LastHighlightedObject = newObject;
            if (audioSource != null && newObject != null)
            {
                audioSource.Play();
            }
        }

        public void UpdateHighlighters()
        {
            foreach (GameObject obj in objectsToHighlight)
            {
                rendererToHighlight[obj] = obj.GetComponentsInChildren<Renderer>();
                if (LastHighlightedObject == null)
                {
                    SwitchHighlight(obj, showEvenIfNotHighlighted, true, false);
                    SwitchHighlight(obj, false, false, true);
                }
            }
        }

        protected virtual void Awake()
        {
            LastHighlightedObject = null;
            UpdateHighlighters();
            if (tapDetector == null)
            {
                tapDetector = FindObjectOfType<GestureDetector>();
            }
            if (objectSelector == null)
            {
                objectSelector = FindObjectOfType<ObjectSelector>();
            }
            audioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            UpdateHighlighters();
        }

        protected virtual void OnEnable()
        {
            if (objectSelector != null)
            {
                // Register to gaze events
                objectSelector.SelectionChanged += OnFocusedObjectChanged;
            }
            if (objectSelector != null)
            {
                UpdateHighlight(objectSelector.Selection);
            }
            if (tapDetector != null)
            {
                tapDetector.Tap += OnSelect;
            }
        }

        protected virtual void OnDisable()
        {
            LastHighlightedObject = null;
            if (objectSelector != null)
            {
                // Unregister to gaze events
                objectSelector.SelectionChanged -= OnFocusedObjectChanged;
            }
            if (tapDetector != null)
            {
                tapDetector.Tap -= OnSelect;
            }
        }

        protected virtual GameObject GetHighlightable(GameObject newObject)
        {
            if (newObject == null)
            {
                return null;
            }
            GameObject found = null;
            foreach (GameObject hlObj in objectsToHighlight)
            {
                if (hlObj == newObject)
                {
                    found = hlObj;
                    break;
                }
                if (hlObj.transform == newObject.transform.parent)
                {
                    found = hlObj;
                    break;
                }
            }
            return found;
        }

        protected virtual void OnSelect()
        {
            if (objectSelector)
            {
                GameObject hlObj = GetHighlightable(objectSelector.Selection);
                if (ObjectSelected != null && hlObj != null)
                {
                    ObjectSelected(hlObj);
                }
            }
        }

        protected virtual void OnFocusedObjectChanged(GameObject previousObject, GameObject newObject)
        {
            UpdateHighlight(newObject);
        }

        /// <summary>
        /// If autoHighlightOnFocus is true highlight the currently targeted object upon getting
        /// an event indicating that the focused object has changed.
        /// </summary>
        /// <param name="newObject">New object being focused.</param>
        protected virtual void UpdateHighlight(GameObject newObject)
        {
            GameObject hlObj = GetHighlightable(newObject);
            if (newObject != null && hlObj == null)
            {
                return;
            }

            if (FocusOnObject != null && newObject != null)
            {
                FocusOnObject(hlObj);
            }
            if (!autoHighlightOnFocus)
            {
                return;
            }
            Highlight(hlObj);
        }

    }
}
