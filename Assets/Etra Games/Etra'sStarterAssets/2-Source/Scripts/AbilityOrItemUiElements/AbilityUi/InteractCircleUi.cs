using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class InteractCircleUi : EtraAbilityOrItemUi
    {
        [Header("Basics")]
        [Range(0, 1f)]
        public float sliderValue = 0.8f;

        [Header("Customization")]
        public Color fillColor = new Color(0.283372f, 0, 0.5960785f);
        public Color backgroundColor = new Color(0.05660379f, 0.05660379f, 0.05660379f);

        [Header("Elements")]
        public Slider slider; // don't filp active
        public TextMeshProUGUI leftText;
        public TextMeshProUGUI rightText;
        public Image fill;
        public Image outerBorder;
        public Image innerBorder;


        private void Reset()
        {
            OnValidate();
        }


        private void OnValidate()
        {
            //Values
            slider.value = sliderValue;

            //Colors
            fill.color = fillColor;
            outerBorder.color = backgroundColor; 
            innerBorder.color = backgroundColor;
        }

        private void Update()
        {
            //Values
            slider.value = sliderValue;
        }
        public void sliderVisibility(bool visibility)
        {
            fill.enabled = visibility;
            outerBorder.enabled = visibility;
            innerBorder.enabled = visibility;
        }

    }
}
