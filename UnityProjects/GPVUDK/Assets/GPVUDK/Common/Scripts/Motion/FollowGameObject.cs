// Code partially derived from an old version of VRTK - Virtual Reality Toolkit
// (https://github.com/thestonefox/VRTK)

using UnityEngine;

namespace GPVUDK
{
    /// <summary>
    /// Class that makes a game object follow another game object.
    /// </summary>
    public class FollowGameObject : MonoBehaviour
    {
        /// <summary>
        /// Moment at which to follow.
        /// </summary>
        /// <param name="OnUpdate"></param>
        /// <param name="OnLateUpdate"></param>
        /// <param name="OnPreRender"></param>
        /// <param name="OnFixedUpdate"></param>
        public enum FollowMoment
        {
            /// <summary>
            /// Automatically select the moment at which to follow
            /// </summary>
            Auto,
            /// <summary>
            /// Follow in the Update method.
            /// </summary>
            OnUpdate,
            /// <summary>
            /// Follow in the LateUpdate method.
            /// </summary>
            OnLateUpdate,
            /// <summary>
            /// Follow in the OnPreRender method (this script doesn't have to be attached to a camera).
            /// </summary>
            OnPreRender,
            /// <summary>
            /// Follow in the OnFixedUpdate method (rigidbodies).
            /// </summary>
            OnFixedUpdate,
            /// <summary>
            /// Follow only if the Follow() method is called by another script (or UI)
            /// </summary>
            Script
        }

        /// <summary>
        /// Follow mode.
        /// </summary>
        public enum FollowMode
        {
            /// <summary>
            /// Automatically changed to the most suitable mode.
            /// </summary>
            Auto,
            /// <summary>
            /// Game object Transform follows another Transform.
            /// </summary>
            TransformToTransform,
            /// <summary>
            /// Game object local Transform follows another local Transform.
            /// </summary>
            LocalTransformToTransform,
            /// <summary>
            /// Game object Rigidbody follows a Transform.
            /// </summary>
            RigidbodyToTransform,
            /// <summary>
            /// Game object Rigidbody follows another Rigidbody.
            /// </summary>
            RigidbodyToRigidbody
        }

        /// <summary>
        /// Specifies how to position and rotate the rigidbody.
        /// </summary>
        public enum MovementOption
        {
            /// <summary>
            /// Use <see cref="Rigidbody.position"/> and <see cref="Rigidbody.rotation"/>.
            /// </summary>
            Set,
            /// <summary>
            /// Use <see cref="Rigidbody.MovePosition"/> and <see cref="Rigidbody.MoveRotation"/>.
            /// </summary>
            Move,
            /// <summary>
            /// Use <see cref="Rigidbody.AddForce(Vector3)"/> and <see cref="Rigidbody.AddTorque(Vector3)"/>.
            /// </summary>
            Add
        }


        [Tooltip("The game object to follow. If null a new object is created and followed (AnchorObject property, accessible via script).")]
        [SerializeField]
        private GameObject gameObjectToFollow;
		
        [Tooltip("The game object to be changed. If left empty it is this game object.")]
        [SerializeField]
        private GameObject gameObjectToChange;
		
        /// <summary>
        /// Follow only if the object is active.
        /// </summary>
        [Tooltip("Follow only if the object is active.")]
        public bool onlyIfActive = false;

        /// <summary>
        /// Whether to follow the position of the given game object.
        /// </summary>
        [Tooltip("Whether to follow the position of the given game object.")]
        public bool followsPosition = true;
		
        /// <summary>
        /// Whether to smooth the position when following `gameObjectToFollow`.
        /// </summary>
        [Tooltip("Whether to smooth the position when following `gameObjectToFollow`.")]
        public bool smoothsPosition = false;
		
        /// <summary>
        /// The maximum allowed distance between the unsmoothed source and the smoothed target per frame to use for smoothing.
        /// </summary>
        [Tooltip("The maximum allowed distance between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
        public float maxAllowedPerFrameDistanceDifference = 0.03f;

        /// <summary>
        /// Whether to follow the rotation of the given game object.
        /// </summary>
        [Tooltip("Whether to follow the rotation of the given game object.")]
        public bool followsRotation = true;
		
        /// <summary>
        /// Whether to smooth the rotation when following `gameObjectToFollow`.
        /// </summary>
        [Tooltip("Whether to smooth the rotation when following `gameObjectToFollow`.")]
        public bool smoothsRotation = false;
		
        /// <summary>
        /// Follow only the heading.
        /// </summary>
        [Tooltip("Follow only the heading")]
        public bool headingOnly = false;
		
        /// <summary>
        /// The maximum allowed angle between the unsmoothed source and the smoothed target per frame to use for smoothing.
        /// </summary>
        [Tooltip("The maximum allowed angle between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
        public float maxAllowedPerFrameAngleDifference = 1.5f;
		
        /// <summary>
        /// Angle threshold in degrees to activate the rotation when following `gameObjectToFollow`.
        /// </summary>
        [Tooltip("Angle threshold in degrees to activate the rotation when following `gameObjectToFollow`.")]
        public float rotationThreshold = 0;

        /// <summary>
        /// Whether to follow the scale of the given game object.
        /// </summary>
        [Tooltip("Whether to follow the scale of the given game object.")]
        public bool followsScale = false;
		
        /// <summary>
        /// Whether to smooth the scale when following `gameObjectToFollow`.
        /// </summary>
        [Tooltip("Whether to smooth the scale when following `gameObjectToFollow`.")]
        public bool smoothsScale = false;
		
        /// <summary>
        /// The maximum allowed size between the unsmoothed source and the smoothed target per frame to use for smoothing.
        /// </summary>
        [Tooltip("The maximum allowed size between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
        public float maxAllowedPerFrameSizeDifference = 0.003f;

        [Header("Advanced")]
        [Tooltip("'Auto' mode switches automaticallly to the proper mode.\n'Rigidbody' modes need Rigidbody components attached to the proper game objects.")]
        [SerializeField]
        private FollowMode followMode = FollowMode.Auto;

        [Tooltip("Moment at which to follow.\nAuto automatically selects the proper moment.")]
        [SerializeField]
        private FollowMoment followMoment = FollowMoment.Auto;

        [Tooltip("How to change position and rotation for 'Rigidbody' modes:\nSet = directly set\nMove = move with interpolation\nAdd = add force/torque to move")]
        [SerializeField]
        private MovementOption movementOption = MovementOption.Move;

        private Transform transformToFollow;
        private Transform transformToChange;
        private Rigidbody rigidbodyToChange;
        private Rigidbody rigidbodyToFollow;

		
        /// <summary>
        /// The position that results by following `gameObjectToFollow`.
        /// </summary>
        public Vector3 TargetPosition { get; private set; }
		

        /// <summary>
        /// Moment at which this game object follows another transform/rigidbody.
        /// </summary>
        public FollowMoment Moment
        {
            get
            {
                return followMoment;
            }
            set
            {
                if (followMoment == value)
                {
                    return;
                }

                if (isActiveAndEnabled)
                {
                    if (value == FollowMoment.OnPreRender)
                    {
                        Camera.onPreRender += OnCamPreRender;
                    }
                    else
                    {
                        Camera.onPreRender -= OnCamPreRender;
                    }
                }
                followMoment = value;
            }
        }

        /// <summary>
        /// The rotation that results by following `gameObjectToFollow`.
        /// </summary>
        public Quaternion TargetRotation { get; private set; }
		
        /// <summary>
        /// The scale that results by following `gameObjectToFollow`.
        /// </summary>
        public Vector3 TargetScale { get; private set; }

        /// <summary>
        /// An anchor object used to "drag" the game object to change.
        /// </summary>
        public GameObject AnchorObject { get; private set; }

        /// <summary>
        /// The game object to follow. If null a new object is created and followed (AnchorObject property).
        /// </summary>
        public GameObject GameObjectToFollow
        {
            get
            {
                return gameObjectToFollow;
            }

            set
            {
                if (AnchorObject != null)
                {
                    Destroy(AnchorObject);
                    AnchorObject = null;
                }
                gameObjectToFollow = value;
            }
        }


        /// <summary>
        /// The game object to be changed. If left empty it is this game object.
        /// </summary>
        public GameObject GameObjectToChange
        {
            get
            {
                return gameObjectToChange;
            }

            set
            {
                gameObjectToChange = value;
            }
        }

		
        /// <summary>
        /// Follow `gameObjectToFollow` using the current settings.
        /// </summary>
        public void Follow()
        {
            CacheAll();
            if (gameObjectToFollow == null)
            {
                return;
            }

            if (onlyIfActive && !gameObjectToFollow.activeInHierarchy)
            {
                return;
            }

            if (followsPosition)
            {
                FollowPosition();
            }

            if (followsRotation)
            {
                FollowRotation();
            }

            if (followsScale)
            {
                FollowScale();
            }
        }

		
        /// <summary>
        /// Initialize internal data and create an anchor object if needed.
        /// </summary>
        public void Init()
        {
            if (gameObjectToFollow == null)
            {
                if (AnchorObject == null)
                {
                    AnchorObject = new GameObject(name + "_ANCHOR");
                    // copy this transform to the anchor object
                    AnchorObject.transform.SetParent(transform, false);
                    AnchorObject.transform.SetParent(transform.parent, true);
                }
                gameObjectToFollow = AnchorObject;
            }
            if (gameObjectToChange == null)
            {
                gameObjectToChange = gameObject;
            }
            InitMode();
            InitMoment();
            CacheAll();
        }


        /// <summary>
        /// Fill the internal references cache
        /// </summary>
        private void CacheAll()
        {
            switch (followMode)
            {
                case FollowMode.TransformToTransform:
                case FollowMode.LocalTransformToTransform:
                    if (gameObjectToFollow == null || gameObjectToChange == null
                        || (transformToFollow != null && transformToChange != null))
                    {
                        return;
                    }

                    transformToFollow = gameObjectToFollow.transform;
                    transformToChange = gameObjectToChange.transform;
                    break;
                case FollowMode.RigidbodyToTransform:
                    if (gameObjectToFollow != null && gameObjectToChange != null)
                    {
                        rigidbodyToChange = gameObjectToChange.GetComponent<Rigidbody>();
                    }
                    if (gameObjectToFollow != null && gameObjectToChange != null
                        && transformToFollow == null)
                    {
                        transformToFollow = gameObjectToFollow.transform;
                    }
                    break;
                case FollowMode.RigidbodyToRigidbody:
                    if (gameObjectToFollow == null || gameObjectToChange == null
                        || (rigidbodyToFollow != null && rigidbodyToChange != null))
                    {
                        return;
                    }

                    rigidbodyToFollow = gameObjectToFollow.GetComponent<Rigidbody>();
                    rigidbodyToChange = gameObjectToChange.GetComponent<Rigidbody>();
                    break;
            }
        }


        private void InitMode()
        {
            if (gameObjectToChange == null || gameObjectToFollow == null)
            {
                Debug.LogError("Game Object To Change or/and Game Object To Follow are missing.");
                return;
            }
            if (!ValidateMode())
            {
                return;
            }
            if (followMode == FollowMode.Auto)
            {
                bool changedHasRB = gameObjectToChange.GetComponent<Rigidbody>() != null;
                bool followedHasRB = gameObjectToFollow.GetComponent<Rigidbody>() != null;
                if (changedHasRB && followedHasRB)
                {
                    followMode = FollowMode.RigidbodyToRigidbody;
                }
                else if (changedHasRB && !followedHasRB)
                {
                    followMode = FollowMode.RigidbodyToTransform;
                }
                else
                {
                    followMode = FollowMode.TransformToTransform;
                }
            }
        }

        private void InitMoment()
        {
            if (gameObjectToFollow != null && followMoment == FollowMoment.Auto)
            {
                if (followMode == FollowMode.RigidbodyToRigidbody || followMode == FollowMode.RigidbodyToTransform)
                {
                    if (movementOption == MovementOption.Set)
                    {
                        Moment = FollowMoment.OnUpdate;
                    }
                    else
                    {
                        Moment = FollowMoment.OnFixedUpdate;
                    }
                    return;
                }
                FollowGameObject followGO = gameObjectToFollow.GetComponentInParent<FollowGameObject>();

                if (followGO != null)
                {
                    followGO.Init();
                }
                Moment = FollowMoment.OnLateUpdate;
            }
        }

        private bool ValidateMode()
        {
            if (gameObjectToChange == null || gameObjectToFollow == null)
            {
                return false;
            }
            if ((followMode == FollowMode.RigidbodyToTransform || followMode == FollowMode.RigidbodyToRigidbody) && gameObjectToChange.GetComponent<Rigidbody>() == null)
            {
                Debug.LogError("Follow Mode cannot be set to Rigidbody To Transform without a rigidbody.\nAdd a Rigidbody to " + gameObjectToChange.name + " or choose another mode.");
                return false;
            }
            if (followMode == FollowMode.RigidbodyToRigidbody && gameObjectToFollow.GetComponent<Rigidbody>() == null)
            {
                Debug.LogError("Follow Mode cannot be set to Rigidbody To Rigidbody without a rigidbody defined for the followed game object.\nAdd a Rigidbody to " + gameObjectToFollow.name + " or choose another mode.");
                return false;
            }
            return true;
        }

        //this method shouldn't be called `OnPreRender` to prevent a name clash with `MonoBehaviour.OnPreRender` since that is used when this script is attached to a camera
        private void OnCamPreRender(Camera cam)
        {
            if (Moment == FollowMoment.OnPreRender)
            {
                Follow();
            }
        }

        private void OnEnable()
        {
            Init();
            if (Moment == FollowMoment.OnPreRender)
            {
                Camera.onPreRender += OnCamPreRender;
            }
            if(AnchorObject!=null)
            {
                AnchorObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            transformToFollow = null;
            transformToChange = null;
            rigidbodyToFollow = null;
            rigidbodyToChange = null;
            if (Moment == FollowMoment.OnPreRender)
            {
                Camera.onPreRender -= OnCamPreRender;
            }
            if (AnchorObject != null)
            {
                if (gameObjectToFollow == AnchorObject)
                {
                    gameObjectToFollow = null;
                }
                Destroy(AnchorObject);
                AnchorObject = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, maxAllowedPerFrameDistanceDifference);
            maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, maxAllowedPerFrameAngleDifference);
            maxAllowedPerFrameSizeDifference = Mathf.Max(0.0001f, maxAllowedPerFrameSizeDifference);
            if (gameObjectToChange == null)
            {
                gameObjectToChange = gameObject;
            }
            if (followMode != FollowMode.Auto)
            {
                ValidateMode();
            }
            if (followMoment == FollowMoment.OnPreRender && GetComponent<Camera>() != null)
            {
                Debug.LogError("On Pre Render cannot be used with a camera");
            }
        }
#endif

        private void OnDestroy()
        {
            if (AnchorObject != null)
            {
                if (gameObjectToFollow == AnchorObject)
                {
                    gameObjectToFollow = null;
                }
                Destroy(AnchorObject);
                AnchorObject = null;
            }
        }

        private void Update()
        {
            if (Moment == FollowMoment.OnUpdate)
            {
                Follow();
            }
        }

        private void LateUpdate()
        {
            if (Moment == FollowMoment.OnLateUpdate)
            {
                Follow();
            }
        }

        private void FixedUpdate()
        {
            if (Moment == FollowMoment.OnFixedUpdate)
            {
                Follow();
            }
        }

        private Vector3 GetPositionToFollow()
        {
            CacheAll();
            switch (followMode)
            {
                case FollowMode.TransformToTransform:
                    return transformToFollow.position;

                case FollowMode.LocalTransformToTransform:
                    return transformToFollow.localPosition;

                case FollowMode.RigidbodyToTransform:
                    return transformToFollow.position;

                case FollowMode.RigidbodyToRigidbody:
                    return rigidbodyToFollow.position;
            }
            return transform.position;
        }

        private void SetPositionOnGameObject(Vector3 newPosition)
        {
            switch (followMode)
            {
                case FollowMode.TransformToTransform:
                    transformToChange.position = newPosition;
                    break;

                case FollowMode.LocalTransformToTransform:
                    transformToChange.localPosition = newPosition;
                    break;

                case FollowMode.RigidbodyToTransform:
                case FollowMode.RigidbodyToRigidbody:
                    switch (movementOption)
                    {
                        case MovementOption.Set:
                            rigidbodyToChange.position = newPosition;
                            break;
                        case MovementOption.Move:
                            rigidbodyToChange.MovePosition(newPosition);
                            break;
                        case MovementOption.Add:
                            // Add a force proportional to the distance from the new position.
                            // TODO: here mass and current velocity should be taken into account
                            rigidbodyToChange.AddForce(newPosition - rigidbodyToChange.position);
                            break;
                    }
                    break;
            }
        }

        private Quaternion GetRotationToFollow()
        {
            switch (followMode)
            {
                case FollowMode.TransformToTransform:
                case FollowMode.RigidbodyToTransform:
                    return transformToFollow.rotation;

                case FollowMode.LocalTransformToTransform:
                    return transformToFollow.localRotation;

                case FollowMode.RigidbodyToRigidbody:
                    return rigidbodyToFollow.rotation;
            }
            return transform.rotation;
        }

        private void SetRotationOnGameObject(Quaternion newRotation)
        {
            switch (followMode)
            {
                case FollowMode.TransformToTransform:
                    transformToChange.rotation = newRotation;
                    break;

                case FollowMode.LocalTransformToTransform:
                    transformToChange.localRotation = newRotation;
                    break;

                case FollowMode.RigidbodyToTransform:
                case FollowMode.RigidbodyToRigidbody:
                    switch (movementOption)
                    {
                        case MovementOption.Set:
                            rigidbodyToChange.rotation = newRotation;
                            break;
                        case MovementOption.Move:
                            rigidbodyToChange.MoveRotation(newRotation);
                            break;
                        case MovementOption.Add:
                            // Add a force proportional to the difference with the new rotation.
                            // TODO: here mass and current angular velocity should be taken into account
                            rigidbodyToChange.AddTorque(newRotation * Quaternion.Inverse(rigidbodyToChange.rotation).eulerAngles);
                            break;
                    }
                    break;
            }
        }

        private Vector3 GetScaleToFollow()
        {
            return gameObjectToFollow.transform.localScale;
        }

        private void SetScaleOnGameObject(Vector3 newScale)
        {
            gameObjectToChange.transform.localScale = newScale;
        }

        private void FollowPosition()
        {
            Vector3 positionToFollow = GetPositionToFollow();
            Vector3 newPosition;

            if (smoothsPosition)
            {
                float alpha = Mathf.Clamp01(Vector3.Distance(TargetPosition, positionToFollow) / maxAllowedPerFrameDistanceDifference);
                newPosition = Vector3.Lerp(TargetPosition, positionToFollow, alpha);
            }
            else
            {
                newPosition = positionToFollow;
            }

            TargetPosition = newPosition;
            SetPositionOnGameObject(TargetPosition);
        }

        private void FollowRotation()
        {
            Quaternion rotationToFollow = GetRotationToFollow();
            if (headingOnly)
            {
                Vector3 eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.y = rotationToFollow.eulerAngles.y;
                rotationToFollow.eulerAngles = eulerAngles;
            }
            if (Quaternion.Angle(TargetRotation, rotationToFollow) < rotationThreshold)
            {
                return;
            }
            Quaternion newRotation;

            if (smoothsRotation)
            {
                float alpha = Mathf.Clamp01(Quaternion.Angle(TargetRotation, rotationToFollow) / maxAllowedPerFrameAngleDifference);
                newRotation = Quaternion.Lerp(TargetRotation, rotationToFollow, alpha);
            }
            else
            {
                newRotation = rotationToFollow;
            }

            TargetRotation = newRotation;
            SetRotationOnGameObject(newRotation);
        }

        private void FollowScale()
        {
            Vector3 scaleToFollow = GetScaleToFollow();
            Vector3 newScale;

            if (smoothsScale)
            {
                float alpha = Mathf.Clamp01(Vector3.Distance(TargetScale, scaleToFollow) / maxAllowedPerFrameSizeDifference);
                newScale = Vector3.Lerp(TargetScale, scaleToFollow, alpha);
            }
            else
            {
                newScale = scaleToFollow;
            }

            TargetScale = newScale;
            SetScaleOnGameObject(newScale);
        }
    }
}