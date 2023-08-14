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


        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            //Values
            rightmostSlider.value = sliderValue;
            leftmostSlider.value = sliderValue;

            //Customization
            if (isRunning)
            {
                movingGuyIcon.sprite = runningGuySprite;
            }
            else
            {
                movingGuyIcon.sprite = walkingGuySprite;
            }

            //Colors
            background.color = backgroundColor;

            background.gameObject.GetComponent<Outline>().effectColor = outlineColor;
            movingGuyIcon.gameObject.GetComponent<Outline>().effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.5f);

            movingGuyIcon.color = fillColor;
            rightSliderFill.color = fillColor;
            leftSliderFill.color = fillColor;
        }

        private void Update()
        {
            //Values
            rightmostSlider.value = sliderValue;
            leftmostSlider.value = sliderValue;
        }

        public void setAsDefaultColors()
        {
            background.gameObject.GetComponent<Outline>().effectColor = outlineColor;
            movingGuyIcon.gameObject.GetComponent<Outline>().effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.5f);

            movingGuyIcon.color = fillColor;
            rightSliderFill.color = fillColor;
            leftSliderFill.color = fillColor;
        }

        public void setOutOfStaminaColors()
        {
            background.gameObject.GetComponent<Outline>().effectColor = noStaminaOutlineColor;
            movingGuyIcon.gameObject.GetComponent<Outline>().effectColor = new Color(noStaminaOutlineColor.r, noStaminaOutlineColor.g, noStaminaOutlineColor.b, 0.5f);

            movingGuyIcon.color = noStaminaFillColor;
            rightSliderFill.color = noStaminaFillColor;
            leftSliderFill.color = noStaminaFillColor;
        }

        public void setGuyIconToSprint(bool sprinting)
        {
            if (sprinting)
            {
                movingGuyIcon.sprite = runningGuySprite;
                isRunning = true;
            }
            else
            {
                movingGuyIcon.sprite = walkingGuySprite;
                isRunning = false;
            }
        }

    }
}
