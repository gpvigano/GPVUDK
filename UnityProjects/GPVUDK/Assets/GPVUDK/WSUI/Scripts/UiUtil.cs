using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GPVUDK
{
    /// <summary>
    /// Collection of utility methods for UI.
    /// </summary>
    public static class UiUtil
    {
        /// <summary>
        /// Call OnPointerClick on the given object.
        /// </summary>
        /// <param name="uiObject">Object to be clicked</param>
        /// <returns>Returns true if the object was clicked, false if not found or not clickable.</returns>
        public static bool Click(GameObject uiObject)
        {
            if (uiObject != null)
            {
                Selectable selectable = uiObject.GetComponentInParent<Selectable>();
                if (selectable != null && selectable.interactable)
                {
                    IPointerClickHandler clickHandler = selectable as IPointerClickHandler;
                    if (clickHandler != null)
                    {
                        Debug.Log("Click " + selectable.name);
                        clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Call OnSubmit on the given object.
        /// </summary>
        /// <param name="uiObject">Target object</param>
        /// <returns>Returns true on success, false if no suitable component was found.</returns>
        public static bool Submit(GameObject uiObject)
        {
            if (uiObject != null)
            {
                Selectable selectable = uiObject.GetComponentInParent<Selectable>();
                if (selectable != null && selectable.interactable)
                {
                    ISubmitHandler submitHandler = selectable as ISubmitHandler;
                    if (submitHandler != null)
                    {
                        Debug.Log("Submit " + selectable.name);
                        submitHandler.OnSubmit(new PointerEventData(EventSystem.current));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Call OnPointerDown on the given object.
        /// </summary>
        /// <param name="uiObject">Target object</param>
        /// <returns>Returns true on success, false if no suitable component was found.</returns>
        public static bool PointerDown(GameObject uiObject)
        {
            if (uiObject != null)
            {
                IPointerDownHandler handler = uiObject.GetComponentInParent<IPointerDownHandler>();
                if (handler != null)
                {
                    UIBehaviour selectable = handler as UIBehaviour;
                    if (selectable != null)
                    {
                        Debug.Log("Down " + selectable.name);
                        handler.OnPointerDown(new PointerEventData(EventSystem.current));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Call OnDrag on the given object.
        /// </summary>
        /// <param name="uiObject">Target object</param>
        /// <returns>Returns true on success, false if no suitable component was found.</returns>
        public static bool PointerDrag(GameObject uiObject)
        {
            if (uiObject != null)
            {
                IDragHandler handler = uiObject.GetComponentInParent<IDragHandler>();
                if (handler != null)
                {
                    UIBehaviour selectable = handler as UIBehaviour;
                    if (selectable != null)
                    {
                        Debug.Log("Drag " + selectable.name);
                        handler.OnDrag(new PointerEventData(EventSystem.current));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Call OnPointerUp on the given object.
        /// </summary>
        /// <param name="uiObject">Target object</param>
        /// <returns>Returns true on success, false if no suitable component was found.</returns>
        public static bool PointerUp(GameObject uiObject)
        {
            if (uiObject != null)
            {
                IPointerUpHandler handler = uiObject.GetComponentInParent<IPointerUpHandler>();
                if (handler != null)
                {
                    UIBehaviour selectable = handler as UIBehaviour;
                    if (selectable != null)
                    {
                        Debug.Log("Up " + selectable.name);
                        handler.OnPointerUp(new PointerEventData(EventSystem.current));
                        return true;
                    }
                }
            }
            return false;
        }
    }
}