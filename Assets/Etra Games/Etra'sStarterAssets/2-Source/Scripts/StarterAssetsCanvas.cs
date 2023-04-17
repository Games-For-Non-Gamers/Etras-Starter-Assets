using System.Collections;
using UnityEngine;

namespace EtrasStarterAssets {
    public class StarterAssetsCanvas : MonoBehaviour
    {
        //Get the child objects
        public RectTransform reticle;
        public RectTransform screenWiper;
        public Vector3 screenWipeStart = new Vector3 (1600,0,0);
        public Vector3 screenWipeEnd = new Vector3(-1600, 0, 0);

        private void Start()
        {
            //Hide the screen swiper at the start of the game if it exists
            if (screenWiper != null)
            {
                screenWiper.gameObject.gameObject.SetActive(false);
                LeanTween.move(screenWiper, screenWipeStart, 0);
            } 
        }

        bool screenWipeIsAnimating = false;
        public void screenWipe(float time)
        {
            if (screenWiper == null) { Debug.LogError("The screen wiper is missing and the animation cannot be played\n" +
            "Please re-add ABILITY_CheckpointRespawn to your character."); return; }
            if (screenWipeIsAnimating) { return; }
            screenWipeIsAnimating = true;
            screenWiper.gameObject.gameObject.SetActive(true);
            StartCoroutine(screenWipeAnimation(time));
        }

        IEnumerator screenWipeAnimation(float time)
        {
            LeanTween.move(screenWiper, screenWipeEnd, time).setEaseInOutSine();
            yield return new WaitForSeconds(time);
            screenWiper.gameObject.gameObject.SetActive(false);
            LeanTween.move(screenWiper, screenWipeStart, 0);
            screenWipeIsAnimating =false;
        }
    }
}
