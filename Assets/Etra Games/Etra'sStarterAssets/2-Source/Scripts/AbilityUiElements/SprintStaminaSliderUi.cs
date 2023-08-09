using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Etra.StarterAssets
{
    public class SprintStaminaSliderUi : MonoBehaviour
    {
        [Header("Basics")]
        [Range(0, 1f)]
        public float sliderValue = 0.8f;
        public bool isRunning = false;

        [Header("Customization")]
        public Color fillColor = new Color(1f, 0.4878509f, 0);
        public Color outlineColor = new Color(1f, 0.8064977f, 0);
        public Color backgroundColor = new Color(0, 0, 0);
        public Sprite walkingGuySprite;
        public Sprite runningGuySprite;

        [Header("Elements")]
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


    }
}
