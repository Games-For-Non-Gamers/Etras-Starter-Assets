using EtrasStarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Etra.StarterAssets.Source
{
    public class StarterAssetsCanvas : MonoBehaviour
    {
        //Get the child objects
        public RectTransform reticle;
        public TextMeshProUGUI fpsCounter;
        public GameObject screenWiper;
        private RectTransform screenWiperRect;
        public Vector3 screenWipeStart = new Vector3(1600, 0, 0);
        public Vector3 screenWipeEnd = new Vector3(-1600, 0, 0);
        public bool fpsCounterOn = false;
        private float deltaTime;


        private void Start()
        {
            //Hide the screen swiper at the start of the game if it exists
            if (screenWiper != null)
            {
                setInitialScreenPosition();
            }
        }

        public void setInitialScreenPosition()
        {
            screenWiperRect = screenWiper.GetComponent<RectTransform>();
            screenWiper.gameObject.SetActive(false);
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
            screenWiper.gameObject.gameObject.SetActive(true);
            StartCoroutine(screenWipeAnimation(time));
        }

        IEnumerator screenWipeAnimation(float time)
        {
            screenWiper.GetComponent<AudioManager>().Play("ScreenWipe");
            LeanTween.move(screenWiperRect, screenWipeEnd, time).setEaseInOutSine();
            yield return new WaitForSeconds(time);
            screenWiper.gameObject.gameObject.SetActive(false);
            screenWiperRect.localPosition = screenWipeStart;
            screenWipeIsAnimating = false;
        }

        private void Update()
        {
            if (fpsCounterOn)
            {
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
                int fps = Mathf.RoundToInt(1.0f / deltaTime);
                fpsCounter.text = "FPS: " + fps;
            }
        }

        public void enableFpsCounter()
        {
            fpsCounterOn = true;
            fpsCounter.enabled = true;
        }

        public void disableFpsCounter()
        {
            fpsCounterOn = false;
            fpsCounter.enabled = false;
        }

    }
}
