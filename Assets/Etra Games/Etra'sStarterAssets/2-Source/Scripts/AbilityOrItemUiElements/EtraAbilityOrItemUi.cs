using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class EtraAbilityOrItemUi : MonoBehaviour
    {
        //Misc
        void disableComponent(MonoBehaviour c)
        {
            c.enabled = false;
        }

        //Immediate hide or show
        public void showUi()
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>()) { image.enabled = true; image.color = new Color(image.color.r, image.color.g, image.color.b, 1); }
            foreach (Text text in transform.GetComponentsInChildren<Text>()) { text.enabled = true; text.color = new Color(text.color.r, text.color.g, text.color.b, 1); }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>()) { tmp.enabled = true; tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1); }
        }            
        public void hideUi()
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>()) { image.enabled = false; }
            foreach (Text text in transform.GetComponentsInChildren<Text>()) { text.enabled = false; }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>()) { tmp.enabled = false; }
        }

        //Fade in or out over time
        public void fadeInUi(float time)
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>())
            {
                image.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = image.color; newColor.a = alphaValue; image.color = newColor; }).setEaseInOutSine();
            }
            foreach (Text text in transform.GetComponentsInChildren<Text>())
            {
                text.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = text.color; newColor.a = alphaValue; text.color = newColor; }).setEaseInOutSine();
            }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                tmp.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = tmp.color; newColor.a = alphaValue; tmp.color = newColor; }).setEaseInOutSine();
            }
        }

        public void fadeOutUi(float time)
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>())
            {
                LeanTween.value(1, 0, time).setOnUpdate((float alphaValue) => { Color newColor = image.color; newColor.a = alphaValue; image.color = newColor; }).setEaseInOutSine().setOnComplete(() => { image.enabled = false; });
            }
            foreach (Text text in transform.GetComponentsInChildren<Text>())
            {
                LeanTween.value(1, 0, time).setOnUpdate((float alphaValue) => { Color newColor = text.color; newColor.a = alphaValue; text.color = newColor; }).setEaseInOutSine().setOnComplete(() => { text.enabled = false; });
            }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                LeanTween.value(1, 0, time).setOnUpdate((float alphaValue) => { Color newColor = tmp.color; newColor.a = alphaValue; tmp.color = newColor; }).setEaseInOutSine().setOnComplete(() => { tmp.enabled = false; });
            }
        }


        //Ignore Timescale variation
        public void fadeInUiIgnoreTimescale(float time)
        {
            foreach (Image image in transform.GetComponentsInChildren<Image>())
            {
                image.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = image.color; newColor.a = alphaValue; image.color = newColor; }).setEaseInOutSine().setIgnoreTimeScale(true);
            }
            foreach (Text text in transform.GetComponentsInChildren<Text>())
            {
                text.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = text.color; newColor.a = alphaValue; text.color = newColor; }).setEaseInOutSine().setIgnoreTimeScale(true);
            }
            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                tmp.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = tmp.color; newColor.a = alphaValue; tmp.color = newColor; }).setEaseInOutSine().setIgnoreTimeScale(true);
            }
        }

        public void fadeOutUiIgnoreTimescale(float time)
        {

            foreach (Image image in transform.GetComponentsInChildren<Image>())
            {
                image.enabled = true;
                Color color = image.color;
                LeanTween.value(this.gameObject, color.a, 0, time).setOnUpdate((float alphaValue) => { color.a = alphaValue; image.color = color; }).setIgnoreTimeScale(true).setOnComplete(() => disableComponent(image));
            }
            foreach (Text text in transform.GetComponentsInChildren<Text>())
            {
                text.enabled = true;
                Color color = text.color;
                LeanTween.value(this.gameObject, color.a, 0, time).setOnUpdate((float alphaValue) => { color.a = alphaValue; text.color = color; }).setIgnoreTimeScale(true).setOnComplete(() => disableComponent(text));
            }

            foreach (TextMeshProUGUI tmp in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                tmp.enabled = true;
                Color color = tmp.color;
                LeanTween.value(this.gameObject, color.a, 0, time).setOnUpdate((float alphaValue) => { color.a = alphaValue; tmp.color = color; }).setIgnoreTimeScale(true).setOnComplete(() => disableComponent(tmp));
            }
        }


    }
}
