using Etra.StarterAssets;
using TMPro;
using UnityEngine;

public class EtraPopup : EtraAbilityOrItemUi
{
    public TextMeshProUGUI popupText;
    public TextMeshProUGUI continueText;
    public void UpdateText(string popup, string continueTxt)
    {
        popupText.text = popup;
        continueText.text = continueTxt;
    }


}
