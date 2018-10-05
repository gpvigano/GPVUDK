using UnityEngine;
namespace GPVUDK
{
    /// <summary>
    /// Script to control a camera in a similar way as the Unity Editor Scene view.
    /// </summary>
    /// <remarks>This script is active in Unity Editor only.</remarks>
    public class EditorLikeCameraController : MonoBehaviour
    {
        [SerializeField]
        private bool enableOnlyInEditor = true;

        private void Awake()
        {
            Debug.Log("Notice: " + GetType().Name + " is active in Unity Editor only.");
        }
        private void Update()
        {
#if (!UNITY_EDITOR)
            if(enableOnlyInEditor)
            {
                return;
            }
#endif
            Vector3 mPos = Input.mousePosition;
            if (mPos.x >= 0 && mPos.x < Screen.width && mPos.y >= 0 && mPos.y < Screen.height)
            {
                float x = Input.GetAxis("Horizontal") * 0.1f;
                float y = 0.0f;
                float z = Input.GetAxis("Vertical") * 0.1f;

                if (Input.GetMouseButton(1))
                {
                    float rx = Input.GetAxis("Mouse Y") * 2.0f;
                    float ry = Input.GetAxis("Mouse X") * 2.0f;

                    Vector3 rot = transform.eulerAngles;
                    rot.x -= rx;
                    rot.y += ry;
                    transform.eulerAngles = rot;
                }
                if (Input.GetMouseButton(2))
                {
                    x -= Input.GetAxis("Mouse X") * 0.1f;
                    y -= Input.GetAxis("Mouse Y") * 0.1f;
                }
                z += Input.GetAxis("Mouse ScrollWheel");

                transform.Translate(x, y, z);
            }
        }
    }
}
