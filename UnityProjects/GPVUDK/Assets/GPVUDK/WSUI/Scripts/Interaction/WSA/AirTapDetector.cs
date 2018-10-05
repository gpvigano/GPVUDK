using UnityEngine;

/// <summary>
/// Detect air taps and route them to OnTap method of GestureDetector, this is active only in WSA.
/// </summary>
#if UNITY_WSA_10_0
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif


namespace GPVUDK.HoloLens
{
    public class AirTapDetector : GestureDetector
    {
        private GestureRecognizer recognizer;

        private void Awake()
        {
            recognizer = new GestureRecognizer();
            recognizer.SetRecognizableGestures(GestureSettings.Tap);
        }


        private void OnEnable()
        {
            recognizer.TappedEvent += OnTappedEvent;
            recognizer.StartCapturingGestures();
        }

        private void OnDisable()
        {
            recognizer.TappedEvent -= OnTappedEvent;
            recognizer.StopCapturingGestures();
        }

        private void OnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            OnTap();
        }
    }
}
#else
namespace GPVUDK.HoloLens
{
    public class AirTapDetector : MonoBehaviour
    {
    }
}
#endif
