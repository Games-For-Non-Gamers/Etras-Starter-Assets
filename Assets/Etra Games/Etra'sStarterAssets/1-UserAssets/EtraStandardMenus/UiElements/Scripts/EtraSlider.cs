using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StandardMenus
{
    public class EtraSlider : MonoBehaviour
    {
        public TextMeshProUGUI sliderText;
        public NumberFormat textNumFormat;
        public bool percentage = false;
        private Slider slider;

        public enum NumberFormat
        {
            NoDecimal,
            OneDecimal,
            TwoDecimal,
            PercentOf0To1Range,
        }

        private void OnEnable()
        {
            // Get reference to Slider component
            slider = GetComponent<Slider>();

            // Add a listener to the slider's value changed event
            slider.onValueChanged.AddListener(UpdateSliderText);
        }

        public void UpdateSliderText(float value)
        {
            string formattedString = "";

            switch (textNumFormat)
            {
                case NumberFormat.NoDecimal:
                    formattedString = value.ToString("0");
                    break;

                case NumberFormat.OneDecimal:
                    formattedString = value.ToString("0.0");
                    break;

                case NumberFormat.TwoDecimal:
                    formattedString = value.ToString("0.00");
                    break;

                case NumberFormat.PercentOf0To1Range:
                    formattedString = value.ToString("0.00");
                    formattedString = (float.Parse(formattedString) * 100).ToString();
                    break;
            }

            if (percentage)
            {
                formattedString += "%";
            }

            // Update the text of the slider
            sliderText.text = formattedString;
        }
    }
}