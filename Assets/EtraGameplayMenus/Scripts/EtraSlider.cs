using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EtraSlider : MonoBehaviour
{
    public TextMeshProUGUI sliderText;
    public NumberFormat textNumFormat;
    public bool percentage = false;
    Slider slider;

    public enum NumberFormat
    {
        NoDecimal,
        OneDecimal,
        TwoDecimal,
    }

    void OnEnable()
    {
        slider = this.GetComponent<Slider>();

        slider.onValueChanged.AddListener((v) => {
            UpdateSliderText(v);
        });
    }

    public void UpdateSliderText(float v)
    {
        string returnedString = "";
        switch (textNumFormat)
        {
            case NumberFormat.NoDecimal:
                returnedString = v.ToString("0");
                break;

            case NumberFormat.OneDecimal:
                returnedString = v.ToString("0.0");
                break;

            case NumberFormat.TwoDecimal:
                returnedString = v.ToString("0.00");
                break;
        }

        if (percentage)
        {
            returnedString += "%";
        }
        sliderText.text = returnedString;
    }
}
