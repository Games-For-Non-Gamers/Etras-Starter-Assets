using EtrasStarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets.Source
{
    public class StarterAssetsCanvas : MonoBehaviour
    {
        //Get the child objects
        public RectTransform reticle;
        public GameObject screenWiper;
        public GameObject healthFilter;
        private RectTransform screenWiperRect;
        public Vector3 screenWipeStart = new Vector3(1600, 0, 0);
        public Vector3 screenWipeEnd = new Vector3(-1600, 0, 0);
        public bool fpsCounterOn = false;
        public TextMeshProUGUI speakerLabel;
        public TextMeshProUGUI dialogueLabel;


        private void Start()
        {
            //Hide the screen swiper at the start of the game if it exists
            if (screenWiper != null)
            {
                setInitialScreenWiperState();
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

        public void setInitialScreenWiperState()
        {
            screenWiperRect = screenWiper.GetComponent<RectTransform>();
            screenWiper.gameObject.SetActive(true);
            screenWiperRect.GetComponent<Image>().enabled = false;
            screenWiperRect.localPosition = screenWipeStart;
        }





        bool screenWipeIsAnimating = false;
        public void screenWipe(float time)
        {
            if (screenWiper == null)
            {
                Debug.LogError("The screen wiper is missing and the animation cannot be played\n" +
            "Please re-add ABILITY_CheckpointRespawn to your character."); return;
            }
            if (screenWipeIsAnimating) { return; }
            screenWipeIsAnimating = true;
            screenWiperRect.GetComponent<Image>().enabled = true;
            StartCoroutine(screenWipeAnimation(time));
        }

        IEnumerator screenWipeAnimation(float time)
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
