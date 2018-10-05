using System;
using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Trigger events when an object is selected/deselected (base class to be specialized).
    /// </summary>
    public class ObjectSelector : MonoBehaviour
    {
        /// <summary>
        /// Current selected object
        /// </summary>
        public GameObject Selection { get; protected set; }
        /// <summary>
        /// True if something is selected.
        /// </summary>
        public bool SomethingSelected { get; protected set; }

        /// <summary>
        /// Event triggered whenever the selection changed.
        /// </summary>
        public event Action<GameObject, GameObject> SelectionChanged;

        protected virtual void OnSelectionChanged(GameObject previousObject, GameObject newObject)
        {
            Selection = newObject;
            if (SelectionChanged != null)
            {
                SelectionChanged(previousObject, newObject);
            }
        }
    }
}
