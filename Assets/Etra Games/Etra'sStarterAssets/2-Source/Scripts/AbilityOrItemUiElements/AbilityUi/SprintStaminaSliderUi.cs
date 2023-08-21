using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class SprintStaminaSliderUi : EtraAbilityOrItemUi
    {
        [Header("Basics")]
        [Range(0, 1f)]
        public float sliderValue = 0.8f;
        public bool isRunning = false;

        [Header("Customization")]
        public Color fillColor = new Color(1f, 0.4878509f, 0);
        public Color outlineColor = new Color(1f, 0.8064977f, 0);
        public Color backgroundColor = new Color(0, 0, 0);
        public Color noStaminaFillColor = new Color(1, 0.05f, 0.05f);
        public Color noStaminaOutlineColor = new Color(1, 0, 0f);
        public Sprite walkingGuySprite;
        public Sprite runningGuySprite;

        [Header("Elements")]
        public GameObject sliderBar;
        public Slider rightmostSlider;
        public Slider leftmostSlider;
        public Image rightSliderFill;
        public Image leftSliderFill;
        public Image background;
        public Image movingGuyIcon;

        // Reset values and customization on script reset.
        private void Reset()
        {
            OnValidate();
        }

        // Update values and customization whenever inspector values change.
        private void OnValidate()
        {
            // Values
            rightmostSlider.value = sliderValue;
            leftmostSlider.value = sliderValue;

            // Customization based on whether running or walking.
            movingGuyIcon.sprite = isRunning ? runningGuySprite : walkingGuySprite;

            // Colors
            background.color = backgroundColor;
            ApplyOutlineColor(background.gameObject.GetComponent<Outline>(), outlineColor);
            ApplyOutlineColor(movingGuyIcon.gameObject.GetComponent<Outline>(), new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.5f));

            movingGuyIcon.color = fillColor;
            rightSliderFill.color = fillColor;
            leftSliderFill.color = fillColor;
        }

        // Update slider values during runtime.
        private void Update()
        {
            rightmostSlider.value = sliderValue;
            leftmostSlider.value = sliderValue;
        }

        // Set colors to default values.
        public void SetAsDefaultColors()
        {
            ApplyOutlineColor(background.gameObject.GetComponent<Outline>(), outlineColor);
            ApplyOutlineColor(movingGuyIcon.gameObject.GetComponent<Outline>(), new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.5f));

            movingGuyIcon.color = fillColor;
            rightSliderFill.color = fillColor;
            leftSliderFill.color = fillColor;
        }

        // Set colors to out of stamina values.
        public void SetOutOfStaminaColors()
        {
            ApplyOutlineColor(background.gameObject.GetComponent<Outline>(), noStaminaOutlineColor);
            ApplyOutlineColor(movingGuyIcon.gameObject.GetComponent<Outline>(), new Color(noStaminaOutlineColor.r, noStaminaOutlineColor.g, noStaminaOutlineColor.b, 0.5f));

            movingGuyIcon.color = noStaminaFillColor;
            rightSliderFill.color = noStaminaFillColor;
            leftSliderFill.color = noStaminaFillColor;
        }

        // Set the guy icon based on sprinting status.
        public void SetGuyIconToSprint(bool sprinting)
        {
            movingGuyIcon.sprite = sprinting ? runningGuySprite : walkingGuySprite;
            isRunning = sprinting;
        }

        // Helper function to apply outline color to an Outline component.
        private void ApplyOutlineColor(Outline outlineComponent, Color color)
        {
            if (outlineComponent != null)
            {
                outlineComponent.effectColor = color;
            }
        }
    }
}
