// Code derived from an old version of VRTK - Virtual Reality Toolkit
// (https://github.com/thestonefox/VRTK)

namespace GPVUDK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;

    /// <summary>
    /// The UI Canvas is used to denote which World Canvases are interactable by a UI Pointer.
    /// </summary>
    /// <remarks>
    /// When the script is enabled it will disable the `Graphic Raycaster` on the canvas and create a custom `UI Graphics Raycaster` and the Blocking Objects and Blocking Mask settings are copied over from the `Graphic Raycaster`.
    /// </remarks>
    public class UIWorldCanvas : MonoBehaviour
    {
        protected BoxCollider canvasBoxCollider;
        protected Rigidbody canvasRigidBody;
        protected Coroutine draggablePanelCreation;
        protected const string CANVAS_DRAGGABLE_PANEL = "GPVUDK_UICANVAS_DRAGGABLE_PANEL";
        protected const string ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT = "GPVUDK_UICANVAS_ACTIVATOR_FRONT_TRIGGER";
        [Tooltip("Build colliders for UI elements, suitable for sparse elements with no background.")]
        [SerializeField]
        private bool buildColliders = false;
        [SerializeField]
        [Tooltip("Set this offset for built colliders (positive=front/over the canvas, negative=back).")]
        private float collidersOffset = 0;

        protected virtual void OnEnable()
        {
            SetupCanvas();
        }

        protected virtual void OnDisable()
        {
            RemoveCanvas();
        }

        protected virtual void OnDestroy()
        {
            RemoveCanvas();
        }

        /// <summary>
        /// Build box colliders for each element of the UI in the given canvas.
        /// </summary>
        /// <param name="canvas">Canvas to be processed.</param>
        protected virtual void BuildColliders(Canvas canvas)
        {
            Graphic[] uiGraphics = canvas.gameObject.GetComponentsInChildren<Graphic>();
            foreach (Graphic uiGraph in uiGraphics)
            {
                Collider coll = uiGraph.GetComponent<Collider>();
                RectTransform rectTr = uiGraph.GetComponent<RectTransform>();
                if (coll == null && rectTr != null && uiGraph.raycastTarget)
                {
                    BoxCollider box = uiGraph.gameObject.AddComponent<BoxCollider>();
                    Vector3 boxSize = Vector3.one;
                    boxSize.x = rectTr.rect.width;
                    boxSize.y = rectTr.rect.height;
                    box.size = boxSize;
                    box.center += Vector3.forward * (collidersOffset / canvas.transform.lossyScale.z);
                }
            }
        }

        protected virtual void SetupCanvas()
        {
            Canvas canvas = GetComponent<Canvas>();

            if (!canvas || canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogError("A UIWorldCanvas requires to be placed on a Canvas that is set to `Render Mode = World Space`.");
                return;
            }

            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRectTransform.sizeDelta;
            //copy public params then disable existing graphic raycaster
            GraphicRaycaster defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            UIWorldGraphicRaycaster customRaycaster = canvas.gameObject.GetComponent<UIWorldGraphicRaycaster>();

            //if it doesn't already exist, add the custom raycaster
            if (!customRaycaster)
            {
                customRaycaster = canvas.gameObject.AddComponent<UIWorldGraphicRaycaster>();
            }

            if (defaultRaycaster && defaultRaycaster.enabled)
            {
                customRaycaster.ignoreReversedGraphics = defaultRaycaster.ignoreReversedGraphics;
                customRaycaster.blockingObjects = defaultRaycaster.blockingObjects;
                defaultRaycaster.enabled = false;
            }

            if (buildColliders)
            {
                BuildColliders(canvas);
            }

            //add a box collider and background image to ensure the rays always hit
            if (!canvas.gameObject.GetComponentInChildren<Collider>())
            {
                Vector2 pivot = canvasRectTransform.pivot;
                float zSize = 0.1f;
                float zScale = zSize / canvasRectTransform.localScale.z;

                canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, zScale);
                canvasBoxCollider.center = new Vector3(canvasSize.x / 2 - canvasSize.x * pivot.x, canvasSize.y / 2 - canvasSize.y * pivot.y, zScale / 2f);
                canvasBoxCollider.center += Vector3.forward * (collidersOffset/canvas.transform.lossyScale.z);
                canvasBoxCollider.isTrigger = true;
            }

            if (!canvas.gameObject.GetComponent<Rigidbody>())
            {
                canvasRigidBody = canvas.gameObject.AddComponent<Rigidbody>();
                canvasRigidBody.isKinematic = true;
            }

            draggablePanelCreation = StartCoroutine(CreateDraggablePanel(canvas, canvasSize));
        }

        protected virtual IEnumerator CreateDraggablePanel(Canvas canvas, Vector2 canvasSize)
        {
            if (canvas && !canvas.transform.Find(CANVAS_DRAGGABLE_PANEL))
            {
                yield return null;

                GameObject draggablePanel = new GameObject(CANVAS_DRAGGABLE_PANEL, typeof(RectTransform));
                draggablePanel.AddComponent<LayoutElement>().ignoreLayout = true;
                draggablePanel.AddComponent<Image>().color = Color.clear;
                draggablePanel.AddComponent<EventTrigger>();
                draggablePanel.transform.SetParent(canvas.transform);
                draggablePanel.transform.localPosition = Vector3.zero;
                draggablePanel.transform.localRotation = Quaternion.identity;
                draggablePanel.transform.localScale = Vector3.one;
                draggablePanel.transform.SetAsFirstSibling();

                draggablePanel.GetComponent<RectTransform>().sizeDelta = canvasSize;
            }
        }

        protected virtual void RemoveCanvas()
        {
            Canvas canvas = GetComponent<Canvas>();

            if (canvas!=null)
            {
                return;
            }

            GraphicRaycaster defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            UIWorldGraphicRaycaster customRaycaster = canvas.gameObject.GetComponent<UIWorldGraphicRaycaster>();
            //if a custom raycaster exists then remove it
            if (customRaycaster!=null)
            {
                Destroy(customRaycaster);
            }

            //If the default raycaster is disabled, then re-enable it
            if (defaultRaycaster!=null && !defaultRaycaster.enabled)
            {
                defaultRaycaster.enabled = true;
            }

            //Check if there is a collider and remove it if there is
            if (canvasBoxCollider!=null)
            {
                Destroy(canvasBoxCollider);
            }

            if (canvasRigidBody!=null)
            {
                Destroy(canvasRigidBody);
            }

            if (draggablePanelCreation!=null)
            {
                StopCoroutine(draggablePanelCreation);
            }
            Transform draggablePanel = canvas.transform.Find(CANVAS_DRAGGABLE_PANEL);
            if (draggablePanel!=null)
            {
                Destroy(draggablePanel.gameObject);
            }

            Transform frontTrigger = canvas.transform.Find(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT);
            if (frontTrigger!=null)
            {
                Destroy(frontTrigger.gameObject);
            }
        }
    }
}