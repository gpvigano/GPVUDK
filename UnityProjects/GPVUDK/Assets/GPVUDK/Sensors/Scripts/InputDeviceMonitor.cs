using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    public class InputDeviceMonitor : MonoBehaviour
    {
        public Text uiText;

        [SerializeField]
        private bool writeAxes = true;
        [SerializeField]
        private bool writeGyro = true;
        [SerializeField]
        private bool writeLocation = true;
        [SerializeField]
        private bool writeCompass = true;
        [SerializeField]
        private bool writeAcceleration = true;
        [SerializeField]
        private bool writeOrientation = true;

        // Use this for initialization
        void Start()
        {
            Input.compass.enabled = true;
            Input.gyro.enabled = true;
            if(writeCompass || writeLocation)
            {
                Input.location.Start();
            }
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 eulerAngles = transform.eulerAngles;
            StringBuilder stringBuilder = new StringBuilder();
            if(writeAxes)
            {
                stringBuilder.AppendFormat("Axes\n{0}, {1}\n", Input.GetAxis("Horizontal").ToString(), Input.GetAxis("Vertical").ToString());
            }
            if (writeGyro)
            {
                stringBuilder.AppendLine("Gyro");
                stringBuilder.AppendFormat(" Gravity\n{0}\n", Input.gyro.gravity);
                stringBuilder.AppendFormat(" Attitude\n{0}\n", Input.gyro.attitude.eulerAngles);
            }
            if (writeLocation)
            {
                switch(Input.location.status)
                {
                    case LocationServiceStatus.Running:
                        stringBuilder.AppendFormat("Location\n{0},{1}\n", Input.location.lastData.latitude, Input.location.lastData.latitude);
                        break;
                    default:
                        stringBuilder.AppendLine("Location n/a");
                        break;
                }
            }
            if (writeCompass)
            {
                stringBuilder.AppendLine("Compass");
                stringBuilder.AppendFormat(" Magnetic = {0:D3}\n", (int)(Input.compass.magneticHeading));
                stringBuilder.AppendFormat(" True = {0:D3}\n", (int)(Input.compass.trueHeading));
            }
            if (writeAcceleration)
            {
                stringBuilder.AppendFormat("Acceleration\n{0:F3}, {1:F3}, {2:F3}\n", Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
            }
            if (writeOrientation)
            {
                stringBuilder.AppendFormat("Orientation\n{0}, {1}, {2}", eulerAngles.x.ToString("F0"), eulerAngles.y.ToString("F0"), eulerAngles.z.ToString("F0"));
            }
            string details = stringBuilder.ToString();
            if(uiText!=null)
            {
                uiText.text = details;
            }
        }
    }
}
