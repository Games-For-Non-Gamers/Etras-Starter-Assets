using Etra.StarterAssets;
using TMPro;
using UnityEngine;

namespace Etra.StarterAssets
{
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
}

