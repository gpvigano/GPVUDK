using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    public class SliderTextUpdater : MonoBehaviour
    {
        [Tooltip("Target slider. In this slider you must add SliderTextUpdater.UpdateText() in response to On Value Changed event.")]
        [SerializeField]
        private Slider targetSlider;
        [Tooltip("Text label to be changed according to the above slider value. Tha slider value will be appended to the label text, e.g. <initial text>: <value>.")]
        [SerializeField]
        private Text sliderLabel;
        [Tooltip("decimals used to display the slider value, unless whole numbers are used.")]
        [SerializeField]
        private uint valueDecimals = 2;

        string labelText = string.Empty;

        private void OnValidate()
        {
            if (targetSlider == null)
            {
                targetSlider = GetComponent<Slider>();
            }
            if (sliderLabel == null)
            {
                sliderLabel = GetComponentInChildren<Text>();
            }           
        }

        private void Start()
        {
            if (sliderLabel != null)
            {
                labelText = sliderLabel.text;
            }
            UpdateText();
        }

        public void UpdateText()
        {
            if (targetSlider != null && sliderLabel != null)
            {
                string valText;
                if (targetSlider.wholeNumbers)
                {
                    valText = targetSlider.value.ToString("F0");
                }
                else
                {
                    valText = targetSlider.value.ToString("F"+ valueDecimals);
                }
                sliderLabel.text = labelText + ": " + valText;
            }
        }
    }
}
