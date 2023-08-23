using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace Etra.StandardMenus
{
    public class EtraStandardMenusManager : MonoBehaviour
    {
        [Header("Options")]
        public bool canFreeze = true;
        public bool showBackground = true;
        public bool editCursor = true;
        public bool inGame = true;
        public bool pauseAndUnpauseAudio = true;
        public AudioMixerGroup[] audioGroupsToPause;


        [Header("References")]
        public GameObject background;
        public GameObject pauseMenu;
        public GameObject gameplayMenu;
        public GameObject graphicsMenu;
        public GameObject audioMenu;

        [HideInInspector]public bool gameFrozen = false;
        GameObject currentlyActiveMenu;


        // Private references
        AudioSource[] allAudioSources;
        EventSystem eventSystem;
#if ENABLE_INPUT_SYSTEM
        PlayerInput _playerInput;
#endif

        void Start()
        {
            // Close menus at start in case they are open in the editor
            EtraStandardMenuSettingsFunctions.LoadGraphicsPlayerPrefs();
#if ENABLE_INPUT_SYSTEM
            SetPlayerInputReferenceVariables();
#endif
            UnfreezeGame();
            CloseMenu(pauseMenu);
            CloseMenu(gameplayMenu);
            CloseMenu(graphicsMenu);
            CloseMenu(audioMenu);
            DisableBackground();
        }

        #region Input For Freeze Event
#if ENABLE_INPUT_SYSTEM
        private InputAction keyboardEscape;
        private InputAction gamepadStart;

        void SetPlayerInputReferenceVariables()
        {
            eventSystem = FindObjectOfType<EventSystem>();

            if (_playerInput == null)
            {
                _playerInput = FindObjectOfType<PlayerInput>();
            }
        }

        void OnEnable()
        {
            // Enable the key action
            keyboardEscape = new InputAction("KeyboardEscape", binding: "<Keyboard>/escape");
            keyboardEscape.Enable();
            gamepadStart = new InputAction("GamepadStart", binding: "<Gamepad>/start");
            gamepadStart.Enable();
           // InputSystem.onActionChange += OnActionChange;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= OnActionChange;
            keyboardEscape.Disable();
            keyboardEscape.Dispose();
            gamepadStart.Disable();
            gamepadStart.Dispose();
        }

        GameObject savedSelectedObject = null;

        string savedControlScheme = "";
        void OnActionChange(object action, InputActionChange change)
        {
            if (!gameFrozen)
            {
                return;
            }

            if (!_playerInput)
            {
                return;
            }

            if (_playerInput.currentControlScheme == null)
            {
                return;
            }

            if (_playerInput.currentControlScheme != savedControlScheme)
            {
                savedControlScheme = _playerInput.currentControlScheme;

                if (savedControlScheme.Contains("Keyboard"))
                {
                    if (eventSystem.currentSelectedGameObject != null)
                    {
                        savedSelectedObject = eventSystem.currentSelectedGameObject;
                    }

                    eventSystem.SetSelectedGameObject(null);
                    if (editCursor)
                    {
                        Cursor.visible = true;
                    }

                }
                else
                {
                    if (editCursor)
                    {
                        Cursor.visible = false;
                    }


                    if (savedSelectedObject != null && savedSelectedObject.gameObject.activeInHierarchy)
                    {
                        eventSystem.SetSelectedGameObject(savedSelectedObject);
                    }
                    else if (currentlyActiveMenu != null)
                    {
                        eventSystem.SetSelectedGameObject(currentlyActiveMenu.GetComponent<EtraStandardMenu>().firstSelectedObject.gameObject);
                    }
                }
            }
        }
#endif

        void Update()
        {
            if (canFreeze && inGame)
            {
#if ENABLE_INPUT_SYSTEM
                if (keyboardEscape.triggered || gamepadStart.triggered)
                {
                    PauseInputResults();
                }
#else
                if (Input.GetKeyDown("escape"))
                {
                    PauseInputResults();
                }
#endif
            }
        }

        public void PauseInputResults()
        {
            eventSystem.SetSelectedGameObject(null);
            FreezeOrUnfreeze();
            if (gameFrozen)
            {
                OpenMenu(pauseMenu);
            }
        }


        #endregion

        #region General Menu Functions
        void OpenMenu(GameObject menu)
        {
            EtraStandardMenu gameplayMenu = menu.GetComponent<EtraStandardMenu>();

            if (currentlyActiveMenu != null)
            {
                CloseMenu(currentlyActiveMenu);
            }
            else
            {
                CloseMenu(pauseMenu);
            }

            menu.SetActive(true);
            currentlyActiveMenu = menu;

#if ENABLE_INPUT_SYSTEM
            if (_playerInput.currentControlScheme.Contains("Keyboard"))
            {
                eventSystem.SetSelectedGameObject(null);
                if (editCursor)
                {
                    Cursor.visible = true;
                }

            }
            else
            {
                eventSystem.SetSelectedGameObject(gameplayMenu.firstSelectedObject.gameObject);
                if (editCursor)
                {
                    Cursor.visible = false;

                }
            }
#endif
        }

        void CloseMenu(GameObject menu)
        {
            menu.gameObject.SetActive(false);
        }
        #endregion

        #region Freeze and Unfreeze
        void FreezeOrUnfreeze()
        {
            if (!canFreeze)
            {
                return;
            }

            if (!gameFrozen)
            {
                FreezeGame(true);
            }
            else if (gameFrozen)
            {
                UnfreezeGame(true);
            }
        }

        public void FreezeGame()
        {
            FreezeGame(false);
        }

        void FreezeGame(bool inPauseMenu)
        {

            if (inPauseMenu)
            {
                EnableBackground();
            }
            if (editCursor)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            gameFrozen = true;
#if ENABLE_INPUT_SYSTEM
            InputSystem.onActionChange += OnActionChange;
            if (inGame)
            {
                Time.timeScale = 0;
                _playerInput.SwitchCurrentActionMap("UI");

                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;

                if (pauseAndUnpauseAudio)
                {
                    allAudioSources = FindObjectsOfType<AudioSource>();
                    foreach (AudioSource a in allAudioSources)
                    {
                        if (a!= null)
                        {
                            foreach (AudioMixerGroup group in audioGroupsToPause)
                            {
                                if (a.outputAudioMixerGroup == group && a.isPlaying)
                                {
                                    a.Pause();
                                    break; // Exit the loop once an AudioSource is found and paused
                                }
                            }
                        }
                    }
                }
            }


#endif
        }

        public void UnfreezeGame()
        {
            UnfreezeGame(false);
        }

        public void UnfreezeGame(bool inPauseMenu)
        {
            eventSystem.SetSelectedGameObject(null);
            if (inPauseMenu)
            {
                DisableBackground();
            }

            if (currentlyActiveMenu != null)
            {
                CloseMenu(currentlyActiveMenu);
            }
            else
            {
                CloseMenu(pauseMenu);
            }
            if (editCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            gameFrozen = false;
#if ENABLE_INPUT_SYSTEM
            if (inGame)
            {
                Time.timeScale = 1;
                _playerInput.SwitchCurrentActionMap("Player");
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
                if (pauseAndUnpauseAudio)
                {
                    allAudioSources = FindObjectsOfType<AudioSource>();
                    foreach (AudioSource a in allAudioSources)
                    {
                        if (a != null)
                        {
                            foreach (AudioMixerGroup group in audioGroupsToPause)
                            {
                                if (a.outputAudioMixerGroup == group)
                                {
                                    a.UnPause();
                                    break; // Exit the loop once an AudioSource is found and unpaused
                                }
                            }
                        }
                    }
                }
            }
            InputSystem.onActionChange -= OnActionChange;
#endif
        }
        #endregion

        #region Background Image
        void EnableBackground()
        {
            if (showBackground)
            {
                background.gameObject.SetActive(true);
                background.GetComponent<Image>().enabled = true;
            }
        }

        void DisableBackground()
        {
            background.gameObject.SetActive(false);
            background.GetComponent<Image>().enabled = false;
        }
        #endregion

        #region Public Menu Functions
        public void OpenPauseMenu()
        {
            OpenMenu(pauseMenu);
        }

        public void OpenGameplayMenu()
        {
            OpenMenu(gameplayMenu);
        }

        public void OpenGraphicsMenu()
        {
            OpenMenu(graphicsMenu);
        }

        public void OpenAudioMenu()
        {
            OpenMenu(audioMenu);
        }

        public void AutoSelectGraphicsQuality()
        {
            EtraStandardMenuSettingsFunctions.AutomaticallySelectQuality();
        }

        #endregion
    }
}
