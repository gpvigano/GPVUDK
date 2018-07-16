// Code derived from an old version of VRTK - Virtual Reality Toolkit
// (https://github.com/thestonefox/VRTK)

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GPVUDK
{
    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="currentTarget">The current UI element that the pointer is colliding with.</param>
    /// <param name="previousTarget">The previous UI element that the pointer was colliding with.</param>
    /// <param name="raycastResult">The raw raycast result of the UI ray collision.</param>
    public struct UIPointerEventArgs
    {
        public GameObject currentTarget;
        public GameObject previousTarget;
        public RaycastResult raycastResult;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="UIPointerEventArgs"/></param>
    public delegate void UIPointerEventHandler(object sender, UIPointerEventArgs e);

    /// <summary>
    /// The UI Pointer provides a mechanism for interacting with Unity UI elements on a world canvas. The UI Pointer can be attached to any game object the same way in which a Base Pointer can be and the UI Pointer also requires a controller to initiate the pointer activation and pointer click states.
    /// </summary>
    /// <remarks>
    /// The simplest way to use the UI Pointer is to attach the script to a game controller along with a Simple Pointer as this provides visual feedback as to where the UI ray is pointing.
    ///
    /// The UI pointer is activated via the `Pointer` alias on the `Controller Events` and the UI pointer click state is triggered via the `UI Click` alias on the `Controller Events`.
    /// </remarks>
    public class UIWorldPointer : MonoBehaviour
    {
        /// <summary>
        /// Methods of activation.
        /// </summary>
        /// <param name="HoldButton">Only activates the UI Pointer when the Pointer button on the controller is pressed and held down.</param>
        /// <param name="ToggleButton">Activates the UI Pointer on the first click of the Pointer button on the controller and it stays active until the Pointer button is clicked again.</param>
        /// <param name="AlwaysOn">The UI Pointer is always active regardless of whether the Pointer button on the controller is pressed or not.</param>
        //public enum ActivationMethods
        //{
        //    HoldButton,
        //    ToggleButton,
        //    AlwaysOn
        //}

        /// <summary>
        /// Methods of when to consider a UI Click action
        /// </summary>
        /// <param name="ClickOnButtonUp">Consider a UI Click action has happened when the UI Click alias button is released.</param>
        /// <param name="ClickOnButtonDown">Consider a UI Click action has happened when the UI Click alias button is pressed.</param>
        public enum ClickMethods
        {
            ClickOnButtonUp,
            ClickOnButtonDown
        }

        [Header("Selection Settings")]

        [Tooltip("Determines when the UI Click event action should happen.")]
        public ClickMethods clickMethod = ClickMethods.ClickOnButtonUp;

        [Header("Activation Settings")]

        [Tooltip("If true and the pointer stays over the same UI element for the given time, it automatically attempts to click it.")]
        public bool enableClickAfterHover = false;
        [Tooltip("The amount of time the pointer can be over the same UI element before it automatically attempts to click it. 0f means no click attempt will be made.")]
        public float clickAfterHoverDuration = 0f;
        [Tooltip("(Optional) If Hover Duration Timer is set this will display the time progress.")]
        public Image progressImage;

        [Header("Customisation Settings")]

        [Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
        public Transform pointerOriginTransform = null;
        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        [HideInInspector]
        public float hoverDurationTimer = 0f;
        [HideInInspector]
        public bool canClickOnHover = false;

        /// <summary>
        /// The GameObject of the front trigger activator of the canvas currently being activated by this pointer.
        /// </summary>
        [HideInInspector]
        public GameObject autoActivatingCanvas = null;
        /// <summary>
        /// Determines if the UI Pointer has collided with a valid canvas that has collision click turned on.
        /// </summary>
        [HideInInspector]
        public bool collisionClick = false;

        /// <summary>
        /// Emitted when the UI Pointer is colliding with a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementEnter;
        /// <summary>
        /// Emitted when the UI Pointer is no longer colliding with any valid UI elements.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementExit;
        /// <summary>
        /// Emitted when the UI Pointer has clicked the currently collided UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementClick;
        /// <summary>
        /// Emitted when the UI Pointer begins dragging a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementDragStart;
        /// <summary>
        /// Emitted when the UI Pointer stops dragging a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementDragEnd;

        protected bool pointerClicked = false;
        protected bool beamEnabledState = false;
        protected bool lastPointerPressState = false;
        protected bool lastPointerClickState = false;
        protected GameObject currentTarget;

        protected EventSystem cachedEventSystem;
        protected UIWorldPointerInputModule cachedVRInputModule;
        protected GestureDetector gestureDetector;

        // for UI connection
        public bool EnableClickAfterHover
        {
            get
            {
                return enableClickAfterHover;
            }

            set
            {
                enableClickAfterHover = value;
            }
        }

        public virtual void OnUIPointerElementEnter(UIPointerEventArgs e)
        {
            if (e.currentTarget != currentTarget)
            {
                ResetHoverTimer();
            }

            Selectable uiSel = e.currentTarget ? e.currentTarget.GetComponent<Selectable>() as Selectable : null;
            if (uiSel!=null && uiSel.interactable && enableClickAfterHover && hoverDurationTimer <= 0f)
            {
                canClickOnHover = true;
                hoverDurationTimer = clickAfterHoverDuration;
            }

            currentTarget = e.currentTarget;
            if (UIPointerElementEnter != null)
            {
                UIPointerElementEnter(this, e);
            }
        }

        public virtual void OnUIPointerElementExit(UIPointerEventArgs e)
        {
            if (e.previousTarget == currentTarget)
            {
                ResetHoverTimer();
            }
            if (UIPointerElementExit != null)
            {
                UIPointerElementExit(this, e);
            }
        }

        public virtual void OnUIPointerElementClick(UIPointerEventArgs e)
        {
            if (e.currentTarget == currentTarget)
            {
                ResetHoverTimer();
            }

            if (UIPointerElementClick != null)
            {
                UIPointerElementClick(this, e);
            }
        }

        public virtual void OnUIPointerElementDragStart(UIPointerEventArgs e)
        {
            if (UIPointerElementDragStart != null)
            {
                UIPointerElementDragStart(this, e);
            }
        }

        public virtual void OnUIPointerElementDragEnd(UIPointerEventArgs e)
        {
            if (UIPointerElementDragEnd != null)
            {
                UIPointerElementDragEnd(this, e);
            }
        }

        public virtual UIPointerEventArgs SetUIPointerEvent(RaycastResult currentRaycastResult, GameObject currentTarget, GameObject lastTarget = null)
        {
            UIPointerEventArgs e;
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            e.raycastResult = currentRaycastResult;
            return e;
        }

        /// <summary>
        /// Set up the global Unity event system for the UI pointer.
        /// It also handles disabling the existing Standalone Input Module
        /// that exists on the EventSystem and adds a custom
        /// GPVUDK-WSUI Event System VR Input component that is required
        /// for interacting with the UI with VR inputs.
        /// </summary>
        /// <param name="eventSystem">The global Unity event system to be used by the UI pointers.</param>
        /// <returns>A custom input module that is used to detect input from VR pointers.</returns>
        public virtual UIWorldPointerInputModule SetEventSystem(EventSystem eventSystem)
        {
            if (!eventSystem)
            {
                Debug.LogError("A UIWorldPointer requires an EventSystem");
                return null;
            }

            if (!(eventSystem is UIWorldEventSystem))
            {
                eventSystem = eventSystem.gameObject.AddComponent<UIWorldEventSystem>();
            }

            return eventSystem.GetComponent<UIWorldPointerInputModule>();
        }

        /// <summary>
        /// The RemoveEventSystem resets the Unity EventSystem back to the original state before the VRTK_VRInputModule was swapped for it.
        /// </summary>
        public virtual void RemoveEventSystem()
        {
            UIWorldEventSystem vrtkEventSystem = FindObjectOfType<UIWorldEventSystem>();

            if (!vrtkEventSystem)
            {
                Debug.LogError("A UIWorldPointer requires an EventSystem");
                return;
            }

            Destroy(vrtkEventSystem);
        }

        /// <summary>
        /// The SelectionButtonActive method is used to determine if the configured selection button is currently in the active state.
        /// </summary>
        /// <returns>Returns true if the selection button is active.</returns>
        public virtual bool SelectionButtonPressed()
        {
            if(gestureDetector != null)
            {
                return gestureDetector.Activated;
            }

            return false;
            //return (controller != null ? controller.IsButtonPressed(selectionButton) : false);
        }

        /// <summary>
        /// The ValidClick method determines if the UI Click button is in a valid state to register a click action.
        /// </summary>
        /// <param name="checkLastClick">If this is true then the last frame's state of the UI Click button is also checked to see if a valid click has happened.</param>
        /// <param name="lastClickState">This determines what the last frame's state of the UI Click button should be in for it to be a valid click.</param>
        /// <returns>Returns true if the UI Click button is in a valid state to action a click, returns false if it is not in a valid state.</returns>
        public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)
        {
            bool controllerClicked = (collisionClick ? collisionClick : SelectionButtonPressed());
            bool result = (checkLastClick ? controllerClicked && lastPointerClickState == lastClickState : controllerClicked);
            lastPointerClickState = controllerClicked;
            return result;
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform position for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform position</returns>
        public virtual Vector3 GetOriginPosition()
        {
            return (pointerOriginTransform ? pointerOriginTransform.position : transform.position);
        }

        /// <summary>
        /// The GetOriginPosition method returns the relevant transform forward for the pointer based on whether the pointerOriginTransform variable is valid.
        /// </summary>
        /// <returns>A Vector3 of the pointer transform forward</returns>
        public virtual Vector3 GetOriginForward()
        {
            return (pointerOriginTransform ? pointerOriginTransform.forward : transform.forward);
        }

        protected virtual void OnEnable()
        {
            //pointerOriginTransform = (pointerOriginTransform == null ? VRTK_SDK_Bridge.GenerateControllerPointerOrigin(gameObject) : pointerOriginTransform);

            ConfigureEventSystem();
            pointerClicked = false;
            lastPointerPressState = false;
            lastPointerClickState = false;
            beamEnabledState = false;
        }

        protected virtual void OnDisable()
        {
            if (cachedVRInputModule && cachedVRInputModule.pointers.Contains(this))
            {
                cachedVRInputModule.pointers.Remove(this);
            }
        }

        protected virtual void ResetHoverTimer()
        {
            hoverDurationTimer = 0f;
            canClickOnHover = false;
            if (progressImage != null)
            {
                progressImage.fillAmount = 0f;
            }
        }

        protected virtual void ConfigureEventSystem()
        {
            if (gestureDetector==null)
            {
                gestureDetector = FindObjectOfType<GestureDetector>();
            }

            if (!cachedEventSystem)
            {
                cachedEventSystem = FindObjectOfType<EventSystem>();
            }

            if (!cachedVRInputModule)
            {
                cachedVRInputModule = SetEventSystem(cachedEventSystem);
            }

            if (cachedEventSystem && cachedVRInputModule)
            {
                if (pointerEventData == null)
                {
                    pointerEventData = new PointerEventData(cachedEventSystem);
                }

                if (!cachedVRInputModule.pointers.Contains(this))
                {
                    cachedVRInputModule.pointers.Add(this);
                }
            }
        }

        protected virtual void Update()
        {
            if (enableClickAfterHover && progressImage != null && hoverDurationTimer > 0 && clickAfterHoverDuration > 0)
            {
                progressImage.fillAmount = 1f - hoverDurationTimer / clickAfterHoverDuration;
            }
        }
    }
}