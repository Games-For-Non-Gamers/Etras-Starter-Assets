using Etra.StarterAssets.Source;
using EtrasStarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class ExeMainMenuManager : MonoBehaviour
    {
        public GameObject titleScreen;
        public bool playMusic = true;
        public GameObject creatorBackground;
        public GameObject[] creatorPages;
        public int currentPageIndex;

        private void Start()
        {
            AudioManager audioManager = GetComponent<AudioManager>();
            if (playMusic)
            {
                audioManager.Play("Music");
            }

        }


        void EnableMenuInteractables(GameObject uiPage)
        {
            ChangeInteractablesState(uiPage, true);
        }

        void DisableMenuInteractables(GameObject uiPage)
        {
            ChangeInteractablesState(uiPage, true);
        }

        void ChangeInteractablesState(GameObject uiPage, bool state)
        {
            Button[] buttons = uiPage.GetComponentsInAllChildren<Button>();
            foreach (Button button in buttons) { button.enabled = state; }

            Toggle[] toggles = uiPage.GetComponentsInAllChildren<Toggle>();
            foreach (Toggle toggle in toggles) { toggle.enabled = state; }

            Slider[] sliders = uiPage.GetComponentsInAllChildren<Slider>();
            foreach (Slider slider in sliders) { slider.enabled = state; }

            Scrollbar[] scrollbars = uiPage.GetComponentsInAllChildren<Scrollbar>();
            foreach (Scrollbar scrollbar in scrollbars) { scrollbar.enabled = state; }

            ScrollRect[] scrollRects = uiPage.GetComponentsInAllChildren<ScrollRect>();
            foreach (ScrollRect scrollRect in scrollRects) { scrollRect.enabled = state; }

            TMP_Dropdown[] tmpDropdowns = uiPage.GetComponentsInAllChildren<TMP_Dropdown>();
            foreach (TMP_Dropdown tmpDropdown in tmpDropdowns) { tmpDropdown.enabled = state; }
        }


        public void ToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

            public void ToCreator()
        {
            UiSoundManager.I.a.Play("UiClick");
            titleScreen.SetActive(false);
            creatorBackground.SetActive(true);
            creatorPages[0].SetActive(true);
            currentPageIndex = 0;
            EnableMenuInteractables(creatorPages[currentPageIndex]);
            LeanTween.moveLocal(creatorPages[0], Vector3.zero, 0);
        }

        public void ToMainMenu()
        {
            UiSoundManager.I.a.Play("UiClick");
            foreach (GameObject page in creatorPages)
            {
                page.SetActive(false);
            }
            currentPageIndex = 0;
            creatorBackground.SetActive(false);
            titleScreen.SetActive(true);

        }


        public void ToCreatorPage(int pageNum)
        {
            UiSoundManager.I.a.Play("UiClick");
            StartCoroutine(SlideTransition(pageNum));
        }

        float slideTransitionTime = 0.5f;
        IEnumerator SlideTransition(int pageNum)
        {
            creatorPages[pageNum].SetActive(true);
            DisableMenuInteractables(creatorPages[currentPageIndex]);
            DisableMenuInteractables(creatorPages[pageNum]);
            if (currentPageIndex < pageNum)
            {
                LeanTween.moveLocal(creatorPages[pageNum], new Vector3(2000, 0, 0), 0);
                LeanTween.moveLocal(creatorPages[pageNum], Vector3.zero, slideTransitionTime).setEaseInOutSine();
                LeanTween.moveLocal(creatorPages[currentPageIndex], new Vector3(-2000, 0, 0), slideTransitionTime);
            }

            if (currentPageIndex > pageNum)
            {
                LeanTween.moveLocal(creatorPages[pageNum], new Vector3(-2000, 0, 0), 0);
                LeanTween.moveLocal(creatorPages[pageNum], Vector3.zero, slideTransitionTime).setEaseInOutSine();
                LeanTween.moveLocal(creatorPages[currentPageIndex], new Vector3(2000, 0, 0), slideTransitionTime);
            }

            yield return new WaitForSeconds(slideTransitionTime);
            LeanTween.moveLocal(creatorPages[currentPageIndex], Vector3.zero, 0);
            EnableMenuInteractables(creatorPages[currentPageIndex]);
            creatorPages[currentPageIndex].SetActive(false);
            EnableMenuInteractables(creatorPages[pageNum]);
            currentPageIndex = pageNum;
        }


    }
}