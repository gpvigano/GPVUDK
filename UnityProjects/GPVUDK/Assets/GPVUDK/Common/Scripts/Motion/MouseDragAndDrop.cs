using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Script for mouse direct interaction.
    /// Only objects with a Draggable script attached can be dragged.
    /// </summary>
    public class MouseDragAndDrop : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Transform of the cursor game object.")]
        private Transform cursorTransform;
		
        [SerializeField]
        [Tooltip("Hide the cursor while dragging.")]
        private bool hideCursorIfDragging = false;
		
        [SerializeField]
        [Tooltip("Hide the cursor while not dragging.")]
        private bool hideCursorIfNotDragging = false;
		
        [SerializeField]
        [Tooltip("Selection button must be held down to drag.")]
        private bool holdDownToDrag = true;
		
        [SerializeField]
        [Tooltip("Mouse wheel can be used to adjust the distance.")]
        private bool enableMouseWheel = true;
		
        [SerializeField]
        [Tooltip("Selection button")]
        private string selectButton = "Fire1";
		
        [SerializeField]
        [Tooltip("Cancel button (dragged object is brought back to its previous position.)")]
        private string cancelButton = "Fire2";
		
        [SerializeField]
        [Tooltip("Distance for the cursor when no object is hit.")]
        private float defaultDistance = 10;
		
        [SerializeField]
        [Tooltip("Snap the cursor transform to the hit object.")]
        private bool snapToObject = false;

        private float hitDistance;
        private bool hit = false;
        private GameObject hitObject = null;
        private GameObject prevHitObject = null;
        private GameObject draggedObject = null;
        private Vector3 initDragPos = Vector3.zero;
        private Vector3 hitPoint = Vector3.zero;
        private float pickDistance = 0;


        /// <summary>
        /// Transform of the cursor game object
        /// </summary>
        public Transform CursorTransform
        {
            get { return cursorTransform; }
        }


        /// <summary>
        /// An object was hit by the cursor.
        /// </summary>
        public bool Hit
        {
            get { return hit; }
        }

        /// <summary>
        /// Object hit by the cursor.
        /// </summary>
        public GameObject HitObject
        {
            get { return hitObject; }
        }

        /// <summary>
        /// Distance at which an object was hit by the cursor.
        /// </summary>
        public float HitDistance
        {
            get { return hitDistance; }
        }

        /// <summary>
        /// Point hit by the cursor.
        /// </summary>
        public Vector3 HitPoint
        {
            get { return hitPoint; }
        }

        /// <summary>
        /// Mouse wheel can be used to adjust the distance.
        /// </summary>
        public bool MouseWheelEnabled
        {
            get { return enableMouseWheel; }
            set { enableMouseWheel = value; }
        }

        /// <summary>
        /// An object is currently draggged.
        /// </summary>
        public bool Dragging
        {
            get { return draggedObject != null; }
        }

        /// <summary>
        /// Snap the cursor transform to the hit object.
        /// </summary>
        public bool SnapToObject
        {
            get { return snapToObject; }
            set { snapToObject = value; }
        }

        /// <summary>
        /// Event triggered when the cursor comes over an object.
        /// </summary>
        public event Action<GameObject> BeginMouseOverObject;

        /// <summary>
        /// Event triggered when the cursor leaves an object.
        /// </summary>
        public event Action<GameObject> EndMouseOverObject;

        /// <summary>
        /// Event triggered when an object is selected.
        /// </summary>
        public event Action<GameObject> ObjectSelected;

		/// <summary>
        /// Enable/disable dragging the given Draggable.
        /// </summary>
        /// <param name="draggable">Draggable component.</param>
        public void ToggleDragging(Draggable draggable)
        {
            if (draggable != null && draggedObject == null)
            {
                Drag(draggable);
            }
            else
            {
                Drop();
            }
        }

        /// <summary>
        /// Drag the given Draggable.
        /// </summary>
        /// <param name="draggable">Draggable component.</param>
        public void Drag(Draggable draggable)
        {
            if (draggable != null && draggedObject == null)
            {
                cursorTransform.gameObject.SetActive(true);
                draggable.StartDragging(cursorTransform);
                initDragPos = draggable.transform.position;
                draggedObject = draggable.gameObject;
                pickDistance = hitDistance;
                cursorTransform.gameObject.SetActive(!hideCursorIfDragging);
            }
        }


        /// <summary>
        /// Drop the currently dragged Draggable (if any).
        /// </summary>
        public void Drop()
        {
            if (draggedObject != null)
            {
                Draggable draggable = draggedObject.GetComponent<Draggable>();
                draggable.StopDragging();
                draggedObject = null;
                cursorTransform.gameObject.SetActive(!hideCursorIfNotDragging);
            }
        }

        /// <summary>
        /// Bring back the currently dragged Draggable (if any) and stop dragging.
        /// </summary>
        public void CancelDragging()
        {
            if (draggedObject != null)
            {
                draggedObject.transform.position = initDragPos;
            }
            Drop();
        }


        protected void OnObjectSelected(GameObject selectedObj)
        {
            if (ObjectSelected != null)
            {
                ObjectSelected(selectedObj);
            }
        }


        private void Start()
        {
            if (hideCursorIfNotDragging)
            {
                cursorTransform.gameObject.SetActive(false);
            }
            Collider[] colliders = cursorTransform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
            pickDistance = defaultDistance;
        }


        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            hitObject = null;
            Draggable draggable = null;
            hit = false;

            if (!Dragging)
            {
                hit = Physics.Raycast(ray, out hitInfo);
                if (hit)
                {
                    hitPoint = hitInfo.point;
                    hitDistance = Vector3.Distance(hitInfo.point, ray.origin);
                    pickDistance = hitDistance;
                    hitObject = hitInfo.collider.gameObject;
                    draggable = hitObject.GetComponent<Draggable>();
                }
                if (prevHitObject != hitObject && EndMouseOverObject != null)
                {
                    EndMouseOverObject(prevHitObject);
                }
                if (hit && BeginMouseOverObject != null)
                {
                    BeginMouseOverObject(hitObject);
                }
                prevHitObject = hitObject;
            }

            if (Input.GetButtonDown(selectButton))
            {
                if (hitObject != null)
                {
                    OnObjectSelected(hitObject);
                }
                if (holdDownToDrag)
                {
                    if (hit)
                    {
                        cursorTransform.position = ray.origin + ray.direction * hitDistance;
                        Drag(draggable);
                    }
                }
                else
                {
                    ToggleDragging(draggable);
                }
            }


            if (Input.GetButtonUp(selectButton) && holdDownToDrag)
            {
                Drop();
                return;
            }

            if (Input.GetButtonDown(cancelButton))
            {
                CancelDragging();
                return;
            }
            if (!Dragging && !hit)
            {
                cursorTransform.position = ray.origin + ray.direction * defaultDistance;
            }
            if (!Dragging && hit)
            {
                cursorTransform.position = ray.origin + ray.direction * hitDistance;
            }

            if (!Dragging && snapToObject && hit)
            {
                cursorTransform.position = hitObject.transform.position;
            }
            else
            {
                if (Dragging || !hideCursorIfNotDragging)
                {
                    pickDistance += Input.mouseScrollDelta.y;
                    cursorTransform.position = ray.origin + ray.direction * pickDistance;
                }
            }
        }


        private void OnEnable()
        {
            cursorTransform.gameObject.SetActive(!hideCursorIfNotDragging);
        }


        private void OnDisable()
        {
            Drop();
            if (cursorTransform != null)
            {
                cursorTransform.gameObject.SetActive(false);
            }
        }
    }
}
