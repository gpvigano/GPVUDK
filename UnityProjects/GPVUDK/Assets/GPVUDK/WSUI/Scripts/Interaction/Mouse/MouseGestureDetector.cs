using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Detect gestures (even emulated with mouse) and trigger events
    /// </summary>
    public class MouseGestureDetector : GestureDetector
    {
        public enum MouseButton { Left = 0, Center = 1, Right = 2 }

        [SerializeField]
        [Tooltip("Movement threshold for dragging")]
        [Range(0, 100f)]
        public float moveThreshold = 0;
        [SerializeField]
        [Tooltip("Mouse button used to interact (click, drag, ...)")]
        private MouseButton interactionMouseButton = 0;

        private bool mouseMoved = false;
        private bool mouseButtonDown = false;
        private bool prevMousePosValid = false;
        private Vector2 prevMousePos = Vector2.zero;

        /// <summary>
        /// An interaction gesture started
        /// </summary>
        public override bool Activated
        {
            get { return mouseButtonDown; }
            protected set { mouseButtonDown = value; }
        }
        /// <summary>
        /// True if there was a movement since last activation
        /// </summary>
        public override bool Moved
        {
            get { return mouseMoved; }
            protected set { mouseMoved = value; }
        }

        protected virtual bool GetInteractionButtonDown()
        {
            return Input.GetMouseButtonDown((int)interactionMouseButton);
        }

        protected virtual bool GetInteractionButtonUp()
        {
            return Input.GetMouseButtonUp((int)interactionMouseButton);
        }

        protected virtual void Update()
        {
            if (Activated)
            {
                Vector3 mPos = Input.mousePosition;
                if (mouseMoved)
                {
                    OnDrag();
                }
                else
                {
                    if (!prevMousePosValid)
                    {
                        if (Vector2.Distance(mPos, prevMousePos) > moveThreshold)
                        {
                            mouseMoved = true;
                        }
                    }
                    else
                    {
                        prevMousePos = mPos;
                        prevMousePosValid = true;
                    }
                }
            }

            if (GetInteractionButtonUp())
            {
                prevMousePosValid = false;
                OnDeactivate();
            }
            if (GetInteractionButtonDown())
            {
                prevMousePosValid = false;
                OnActivate();
            }
        }
    }
}
