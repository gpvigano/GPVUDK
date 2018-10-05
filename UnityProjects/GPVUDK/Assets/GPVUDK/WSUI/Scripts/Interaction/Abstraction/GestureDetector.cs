using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Detect gestures and trigger events (this is a base class to be specialized)
    /// </summary>
    public class GestureDetector : MonoBehaviour
    {
        /// <summary>
        /// Event triggered on "tap" gesture
        /// </summary>
        public event Action Tap;
        /// <summary>
        /// Event triggered when the interaction gesture starts
        /// </summary>
        public event Action Activate;
        /// <summary>
        /// Event triggered when the interaction gesture ends
        /// </summary>
        public event Action Deactivate;
        /// <summary>
        /// Event triggered on "drag" gesture
        /// </summary>
        public event Action Drag;
        /// <summary>
        /// An interaction gesture started
        /// </summary>
        public virtual bool Activated { get; protected set; }
        /// <summary>
        /// True if there was a movement since last activation
        /// </summary>
        public virtual bool Moved { get; protected set; }

        protected virtual void OnActivate()
        {
            Moved = false;
            Activated = true;
            if (Activate != null)
            {
                Activate();
            }
        }

        protected virtual void OnDeactivate()
        {
            Activated = false;
            if (Deactivate != null)
            {
                Deactivate();
            }
            if (!Moved)
            {
                OnTap();
            }
            Moved = false;
        }

        protected virtual void OnTap()
        {
                if (Tap!=null)
                {
                    Tap();
                }
        }

        protected virtual void OnDrag()
        {
                if (Drag!=null)
                {
                    Drag();
                }
        }
    }
}
