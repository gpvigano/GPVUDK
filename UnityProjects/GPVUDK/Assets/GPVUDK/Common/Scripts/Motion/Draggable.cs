using System;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Script used to manipulate objects, attach this to an object to make it moveable.
    /// </summary>
    /// <remarks>This script requies FollowGameObject to work,
    /// you should configure its parameters, but GameObjectToFollow
    /// property is set automatically.</remarks>
    [RequireComponent(typeof(FollowGameObject))]
    public class Draggable : MonoBehaviour
    {
        [Tooltip("Toggle draggable behaviour on or off (e.g. to lock the object)")]
        [SerializeField]
        private bool isDraggingEnabled = true;

        private bool isDragging;
        private GameObject anchorObject;
        private FollowGameObject gameObjectFollower;

        /// <summary>
        /// Toggle draggable behaviour on or off (e.g. to lock the object)
        /// </summary>
        public bool IsDraggingEnabled
        {
            get
            {
                return isDraggingEnabled;
            }
            set
            {
                isDraggingEnabled = value;
            }
        }

        /// <summary>
        /// Event triggered when dragging starts.
        /// </summary>
        public event Action StartedDragging;

        /// <summary>
        /// Event triggered when dragging stops.
        /// </summary>
        public event Action StoppedDragging;

        /// <summary>
        /// Starts dragging the object.
        /// </summary>
        public void StartDragging(Transform draggerTransform)
        {
            if (!isDraggingEnabled)
            {
                return;
            }

            if (isDragging)
            {
                return;
            }

            anchorObject = new GameObject(name + "_ANCHOR");
            anchorObject.transform.SetParent(gameObjectFollower.GameObjectToChange.transform, false);
            anchorObject.transform.SetParent(draggerTransform, true);
            anchorObject.transform.localScale = Vector3.one;

            gameObjectFollower.GameObjectToFollow = anchorObject;
            gameObjectFollower.enabled = true;

            isDragging = true;

            if(StartedDragging!=null)
            {
                StartedDragging();
            }
        }

        /// <summary>
        /// Stops dragging the object.
        /// </summary>
        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }

            gameObjectFollower.GameObjectToFollow = null;
            gameObjectFollower.enabled = false;
            Destroy(anchorObject);
            anchorObject = null;

            isDragging = false;
            if(StoppedDragging!=null)
            {
                StoppedDragging();
            }
        }


        /// <summary>
        /// Enables or disables dragging.
        /// </summary>
        /// <param name="isEnabled">Indicates whether dragging should be enabled or disabled.</param>
        public void SetDragging(bool isEnabled)
        {
            if (isDraggingEnabled == isEnabled)
            {
                return;
            }

            isDraggingEnabled = isEnabled;

            if (isDragging)
            {
                StopDragging();
            }
        }

        private void Awake()
        {
            gameObjectFollower = GetComponent<FollowGameObject>();
        }

        private void Start()
        {
            gameObjectFollower.enabled = false;
        }

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }
        }
    }
}
