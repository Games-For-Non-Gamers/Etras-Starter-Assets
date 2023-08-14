using Codice.Client.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class EtraAbilityOrItemUi : MonoBehaviour
    {
        public void showUi()
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>()) { image.enabled = true; LeanTween.color(image.rectTransform, new Color(image.color.r, image.color.g, image.color.b, 1), 0); }
            foreach (Text text in transform.GetComponentsInChildren<Text>()) { text.enabled = true; LeanTween.colorText(text.rectTransform, new Color(text.color.r, text.color.g, text.color.b, 1), 0).setEaseInOutSine(); }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>()) { tmp.enabled = true; LeanTween.alphaText(tmp.rectTransform, 1, 0).setEaseInOutSine(); }
        }
        public void hideUi()
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>()) { image.enabled = false; }
            foreach (Text text in transform.GetComponentsInChildren<Text>()) { text.enabled = false; }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>()) { tmp.enabled = false; }
        }

        public void fadeInUi(float time)
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>())
            {
                image.enabled = true;
                LeanTween.color(image.rectTransform, new Color(image.color.r, image.color.g, image.color.b, 1), time).setEaseInOutSine();
            }
            foreach (Text text in transform.GetComponentsInChildren<Text>()) {
                text.enabled = true;
                LeanTween.colorText(text.rectTransform, new Color(text.color.r, text.color.g, text.color.b, 1), time).setEaseInOutSine();
            }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>()) {
                tmp.enabled = true;
                LeanTween.alphaText(tmp.rectTransform, 1, time).setEaseInOutSine();
            }
        }

        public void fadeOutUi(float time)
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>())
            {
                LeanTween.color(image.rectTransform, new Color(image.color.r, image.color.g, image.color.b, 0), time).setEaseInOutSine();
                image.enabled = false;
            }
            foreach (Text text in transform.GetComponentsInChildren<Text>())
            {
                LeanTween.colorText(text.rectTransform, new Color(text.color.r, text.color.g, text.color.b, 0), time).setEaseInOutSine();
                text.enabled = false;
            }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                LeanTween.colorText(tmp.rectTransform, new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0), time).setEaseInOutSine();
                tmp.enabled = false;
            }
        }
    }
}
