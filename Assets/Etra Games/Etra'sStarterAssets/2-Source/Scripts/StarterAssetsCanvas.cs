using EtrasStarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets.Source
{
    public class StarterAssetsCanvas : MonoBehaviour
    {
        // Child objects
        public RectTransform reticle;
        public GameObject screenWiper;
        public GameObject healthFilter;
        private RectTransform screenWiperRect;
        public Vector3 screenWipeStart = new Vector3(1600, 0, 0);
        public Vector3 screenWipeEnd = new Vector3(-1600, 0, 0);
        public bool fpsCounterOn = false;
        public TextMeshProUGUI speakerLabel;
        public TextMeshProUGUI dialogueLabel;
        public Image popupFadeBackground;

        private void Start()
        {
            // Hide the objects at the start of the game if they exist
            if (screenWiper != null)
            {
                SetInitialScreenWiperState();
            }

            if (speakerLabel != null)
            {
                speakerLabel.enabled = false;
            }

            if (dialogueLabel != null)
            {
                dialogueLabel.enabled = false;
            }
        }

        public void SetInitialScreenWiperState()
        {
            screenWiperRect = screenWiper.GetComponent<RectTransform>();
            screenWiper.gameObject.SetActive(true);
            screenWiperRect.GetComponent<Image>().enabled = false;
            screenWiperRect.localPosition = screenWipeStart;
        }

        private bool screenWipeIsAnimating = false;

        // Perform a screen wipe animation
        public void ScreenWipe(float time)
        {
            if (screenWiper == null)
            {
                Debug.LogError("The screen wiper is missing, and the animation cannot be played. Please re-add ABILITY_CheckpointRespawn to your character.");
                return;
            }

            if (screenWipeIsAnimating)
            {
                return;
            }

            screenWipeIsAnimating = true;
            screenWiperRect.GetComponent<Image>().enabled = true;
            StartCoroutine(ScreenWipeAnimation(time));
        }

        // Coroutine for the screen wipe animation
        private IEnumerator ScreenWipeAnimation(float time)
        {
            screenWiper.GetComponent<AudioManager>().Play("ScreenWipe");
            LeanTween.move(screenWiperRect, screenWipeEnd, time).setEaseInOutSine();
            yield return new WaitForSeconds(time);
            screenWiperRect.GetComponent<Image>().enabled = false;
            screenWiperRect.localPosition = screenWipeStart;
            screenWipeIsAnimating = false;
        }
    }
}
