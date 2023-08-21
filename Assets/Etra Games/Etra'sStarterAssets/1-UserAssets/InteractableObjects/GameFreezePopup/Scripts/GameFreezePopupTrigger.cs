using Etra.StandardMenus;
using Etra.StarterAssets.Source;
using EtrasStarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Etra.StarterAssets
{
    public class GameFreezePopupTrigger : MonoBehaviour
    {
        //public
        public string defaultPopupSfx;
        public List<GameFreezeEntry> eventList;

        //private
        int currentEventNum = -1;
        private bool[] inputPressed;
        private int inputsPressedCount = 0;
        GameObject currentPopup;
        GameFreezeEntry savedEntry;

        //references
        AudioManager audioManager;
        EtraStandardMenusManager menuManager;
        StarterAssetsCanvas starterCanvas;
        float fadeAmount;
        PlayerInput _playerInput;
        EtraCharacterMainController mainController;

        private void Start()
        {
            audioManager = GetComponent<AudioManager>();
            mainController = FindAnyObjectByType<EtraCharacterMainController>();
            starterCanvas = mainController.starterAssetCanvas;
            fadeAmount = starterCanvas.popupFadeBackground.color.a;
            menuManager = FindAnyObjectByType<EtraStandardMenusManager>();
            if (_playerInput == null)
            {
                _playerInput = FindObjectOfType<PlayerInput>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                hidePickup();
                playNextPopupEvent();
            }
        }

        void hidePickup()
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }

        //Cycle through the full eventList
        public void playNextPopupEvent()
        {
            currentEventNum++;
            if (currentEventNum < eventList.Count)
            {
                StartCoroutine(playNextPopupEventCoroutine(currentEventNum));
            }
            else
            {
                StopAllCoroutines();
                // You can destroy the GameObject here if needed
                 Destroy(gameObject);
            }
        }



        IEnumerator playNextPopupEventCoroutine(int entryNum) //different trigger setup
        {
            GameFreezeEntry entry = eventList[entryNum];
            if (currentPopup != null)
            {
                if (currentPopup.GetComponent<EtraPopup>())
                {
                    if (entry.playDefaultAnimation)
                    {
                        currentPopup.GetComponent<EtraPopup>().fadeOutUiIgnoreTimescale(0.1f);
                        yield return new WaitForSecondsIgnoreTimeScale(0.1f);
                    }
                    Destroy(currentPopup);
                }
            }

            switch (entry.chosenEvent)
            {

                case GameFreezeEntry.GameFreezeEvents.Freeze:
                    audioManager.stopAllSounds();
                    mainController.disableAllActiveAbilities();
                    bool cursorState = menuManager.editCursor;
                    menuManager.editCursor = false;
                    menuManager.FreezeGame();
                    menuManager.editCursor = cursorState;
                    menuManager.canFreeze = false;
                    _playerInput.SwitchCurrentActionMap("Player");

                    // Don't pause the audio on this component so it can still play
                    AudioSource[] sources = this.GetComponents<AudioSource>();
                    foreach (AudioSource audioSource in sources)
                    {
                        audioSource.UnPause();
                    }

                    starterCanvas.popupFadeBackground.enabled = true;
                    Color color = starterCanvas.popupFadeBackground.color;
                    LeanTween.value(this.gameObject, color.a, 0, 0).setOnUpdate((float alphaValue) => { color.a = alphaValue; starterCanvas.popupFadeBackground.color = color; }).setIgnoreTimeScale(true);
                    color = starterCanvas.popupFadeBackground.color;
                    LeanTween.value(this.gameObject, color.a, fadeAmount, entry.backgroundFadeTime).setOnUpdate((float alphaValue) => { color.a = alphaValue; starterCanvas.popupFadeBackground.color = color; }).setIgnoreTimeScale(true);
                    yield return new WaitForSecondsIgnoreTimeScale(entry.backgroundFadeTime);
                    playNextPopupEvent();
                    break;

                case GameFreezeEntry.GameFreezeEvents.Popup:
                    currentPopup = Instantiate(entry.popupToAdd.gameObject, Vector3.zero, Quaternion.identity);
                    currentPopup.transform.SetParent(starterCanvas.popupFadeBackground.transform.parent);
                    currentPopup.transform.SetAsLastSibling();
                    savedEntry = entry;
                    currentPopup.transform.localPosition = entry.position;
                    UpdatePopupText();

                    if (entry.playDefaultAudio) { audioManager.Play(defaultPopupSfx); }

                    if (entry.playDefaultAnimation)
                    {
                        currentPopup.transform.localScale = Vector3.zero;
                        LeanTween.scale(currentPopup, Vector3.one, 0.25f).setEaseOutBack().setIgnoreTimeScale(true);
                    }

                    switch (entry.advanceType)
                    {
                        case GameFreezeEntry.AdvanceType.Time:
                            yield return new WaitForSecondsIgnoreTimeScale(entry.timeToWait);
                            playNextPopupEvent();
                            break;
                        case GameFreezeEntry.AdvanceType.WaitForInput:
                            yield return new WaitTillButtonsPressed(entry.inputsNeededToAdvance);
                            playNextPopupEvent();
                            break;
                        case GameFreezeEntry.AdvanceType.External:
                            break;
                    }

                    break;

                case GameFreezeEntry.GameFreezeEvents.AdditionalSfx:
                    audioManager.stopAllSounds();
                    foreach (string sfx in entry.sfxToPlay)
                    {
                        audioManager.Play(sfx);
                    }
                    playNextPopupEvent();
                    break;

                case GameFreezeEntry.GameFreezeEvents.LockMouse:
                    _playerInput.SwitchCurrentActionMap("Player");

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    playNextPopupEvent();
                    break;

                case GameFreezeEntry.GameFreezeEvents.UnlockMouse:
                    _playerInput.SwitchCurrentActionMap("UI");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    playNextPopupEvent();
                    break;

                case GameFreezeEntry.GameFreezeEvents.Unfreeze:
                    audioManager.stopAllSounds();
                    mainController.enableAllActiveAbilities();
                    menuManager.UnfreezeGame();
                    menuManager.canFreeze = true;
                    Color color1 = starterCanvas.popupFadeBackground.color;
                    LeanTween.value(this.gameObject, color1.a, 0, entry.backgroundFadeTime).setOnUpdate((float alphaValue) => { color1.a = alphaValue; starterCanvas.popupFadeBackground.color = color1; }).setIgnoreTimeScale(true);
                    yield return new WaitForSecondsIgnoreTimeScale(entry.backgroundFadeTime);
                    starterCanvas.popupFadeBackground.enabled = false;
                    playNextPopupEvent();
                    break;

                case GameFreezeEntry.GameFreezeEvents.WaitForTime:
                    if (menuManager.gameFrozen)
                    {
                        yield return new WaitForSecondsIgnoreTimeScale(entry.timeToWait);
                    }
                    else
                    {
                        yield return new WaitForSeconds(entry.timeToWait);
                    }
                    playNextPopupEvent();
                    break;

                case GameFreezeEntry.GameFreezeEvents.WaitForInput:
                    yield return new WaitTillButtonsPressed(entry.inputsNeededToAdvance);
                    playNextPopupEvent();
                    break;
            }
        }

        private void UpdatePopupText()
        {
            if (currentPopup.GetComponent<EtraPopup>() && savedEntry != null)
            {
                currentPopup.GetComponent<EtraPopup>().popupText.text = savedEntry.popupText;
                currentPopup.GetComponent<EtraPopup>().popupText.fontSize = savedEntry.popupTextSize;

                if (savedEntry.advanceType == GameFreezeEntry.AdvanceType.WaitForInput)
                {
                    UpdatePopupControllerText(savedEntry.inputsNeededToAdvance);
                }
                else if (savedEntry.advanceType == GameFreezeEntry.AdvanceType.Time)
                {
                    currentPopup.GetComponent<EtraPopup>().continueText.text = "Will continue in " + savedEntry.timeToWait + " seconds...";
                }
                else
                {
                    currentPopup.GetComponent<EtraPopup>().continueText.text = "";
                }
            }
        }

        private void UpdatePopupControllerText(InputActionReference[] inputsNeededToAdvance)
        {
            string continueText = "";
            continueText += "Press ";

            for (int i = 0; i < inputsNeededToAdvance.Length; i++)
            {
                if (inputsNeededToAdvance.Length == 2 && i > 0)
                {
                    continueText += " and ";
                }

                if (inputsNeededToAdvance.Length > 2 && i > 0)
                {
                    continueText += ", and ";
                }

                InputActionReference actionReference = inputsNeededToAdvance[i];

                // Get the InputAction associated with the InputActionReference.
                InputAction action = actionReference.action;

                // Create a list to store the names of buttons tied to this action.
                List<string> buttonNames = new List<string>();

                // Iterate through all controls in the action.
                foreach (InputControl control in action.controls)
                {
                    if (control is ButtonControl buttonControl)
                    {
                        string buttonName = buttonControl.name;
                        buttonNames.Add(buttonName);
                    }
                }

                continueText += string.Join(", or ", buttonNames.ToArray());
            }
            continueText += " to continue...";

            currentPopup.GetComponent<EtraPopup>().continueText.text = continueText;
        }


        private void WaitForInput(InputActionReference[] inputsNeededToAdvance)
        {

            inputPressed = new bool[inputsNeededToAdvance.Length];
            int numberOfInputs = inputsNeededToAdvance.Length;
            inputsPressedCount = 0;

            // Register a callback for each input in the array.
            for (int i = 0; i < numberOfInputs; i++)
            {
                int currentIndex = i;
                inputsNeededToAdvance[i].action.performed += ctx => InputPerformed(currentIndex, ref inputsPressedCount);
                inputsNeededToAdvance[i].action.canceled += ctx => InputCanceled(currentIndex, ref inputsPressedCount);
            }

            // Entry may be changing
            StartCoroutine(WaitForAllInputs(numberOfInputs));
        }

        //May not work in frozen time
        private void InputPerformed(int index, ref int inputsPressedCount)
        {
            inputPressed[index] = true;
            inputsPressedCount++;

            if (inputsPressedCount == inputPressed.Length)
            {
                // All inputs have been pressed simultaneously.
                playNextPopupEvent();
            }
        }

        private void InputCanceled(int index, ref int inputsPressedCount)
        {
            inputPressed[index] = false;
            inputsPressedCount--;
        }

        private IEnumerator WaitForAllInputs(int numberOfInputs)
        {
            if (menuManager.gameFrozen)
            {
                while (inputsPressedCount < numberOfInputs)
                {
                    yield return new WaitForSecondsRealtime(0.01f);
                }
            }
            else
            {
                // Wait until all inputs are pressed.
                while (inputsPressedCount < numberOfInputs)
                {
                    yield return null;
                }
            }
            // Reset pressed inputs and inputsPressedCount.
            for (int i = 0; i < inputPressed.Length; i++)
            {
                inputPressed[i] = false;
            }
            inputsPressedCount = 0;

        }
    }

}
