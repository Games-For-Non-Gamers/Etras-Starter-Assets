using Etra.StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class EtraStandardMenusManager : MonoBehaviour
{

    [Header("Options")]
    public bool canFreeze = true;
    public bool showBackground = true;

    [Header("References")]
    public GameObject background;
    public GameObject pauseMenu;
    public GameObject gameplayMenu;
    public GameObject graphicsMenu;
    public GameObject audioMenu;

    bool gameFrozen = false;
    GameObject currentlyActiveMenu;

    //private references
    EventSystem eventSystem;

    void Start() {
        //Close menus at start in case they are open in editor
        EtraStandardMenuSettingsFunctions.LoadGraphicsPlayerPrefs();
        SetPlayerInputReferenceVariables();
        UnfreezeGame();
        closeMenu(pauseMenu);
        closeMenu(gameplayMenu);
        closeMenu(graphicsMenu);
        closeMenu(audioMenu);
        disableBackground();
        //Set up player input

    }


    #region Input For Freeze Event
    #if ENABLE_INPUT_SYSTEM
    //************** INPUTS/CONTROLLER OR KEYBOARD SELECTION FOR FREEZE EVENT************** 
    public PlayerInput _playerInput = null;
    private void SetPlayerInputReferenceVariables()
    {
        eventSystem = GetComponentInChildren<EventSystem>();

        if (_playerInput == null)
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }
    }

    private InputAction keyboardEscape;
    private InputAction gamepadStart;
    void OnEnable()
    {

        // Enable the key action
        keyboardEscape = new InputAction("KeyboardEscape", binding: "<Keyboard>/escape");
        keyboardEscape.Enable();
        gamepadStart = new InputAction("GamepadStart", binding: "<Gamepad>/start");
        gamepadStart.Enable();
        //Subscribe to OnDeviceChange function

        InputSystem.onDeviceChange += OnDeviceChange;

    }

    void OnDisable()
    {
        keyboardEscape.Disable();
        keyboardEscape.Dispose();
        gamepadStart.Disable();
        gamepadStart.Dispose();
    }

    GameObject savedSelectedObject = null;
    void Update()
    {
        if (canFreeze)
        {
            if (keyboardEscape.triggered || gamepadStart.triggered)
            {
                eventSystem.SetSelectedGameObject(null);
                FreezeOrUnfreeze();
                if (gameFrozen)
                {
                    openMenu(pauseMenu);
                }
            }

        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        //If the game is paused check if selection should be with mouse/keyboard or gamepad
        if (gameFrozen)
        {
            // we want to run this on controller swap
            if (device.name.Contains("Key"))
            {
                if (eventSystem.currentSelectedGameObject != null)
                {
                    savedSelectedObject = eventSystem.currentSelectedGameObject;
                }

                eventSystem.SetSelectedGameObject(null);
                Cursor.visible = true;
            }
            else //Gameplad and mobile
            {
                Cursor.visible = false;
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

    //************************************************************* 
    #endif
    #endregion

    #region GeneralMenuFunctions
    void openMenu(GameObject menu)
    {

        EtraStandardMenu gameplayMenu = menu.GetComponent<EtraStandardMenu>();

        if (currentlyActiveMenu != null)
        {
            closeMenu(currentlyActiveMenu);
        }
        else
        {
            closeMenu(pauseMenu);
        }

        menu.SetActive(true);
        currentlyActiveMenu = menu;

        #if ENABLE_INPUT_SYSTEM
        //If we are using gamepad select the new higlighted button
        if (_playerInput.currentControlScheme.Contains("KeyboardMouse"))
        {
            eventSystem.SetSelectedGameObject(null);
            Cursor.visible = true;
        }
        else 
        {
            eventSystem.SetSelectedGameObject(gameplayMenu.firstSelectedObject.gameObject);
            Cursor.visible = false;
        }
        #endif 
    }

    void closeMenu(GameObject menu)
    {
        menu.gameObject.SetActive(false);
    }

 
    #endregion

    #region FreezeAndUnFreeze
    void FreezeOrUnfreeze()
    {
        if (!canFreeze)
        {
            return;
        }

        if (!gameFrozen)
        {
            FreezeGame();
        } 
        else if (gameFrozen)
        {
            UnfreezeGame();
        }
    }

    void FreezeGame()
    {
        enableBackground();
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        gameFrozen = true;
        _playerInput.SwitchCurrentActionMap("UI");
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    public void UnfreezeGame()
    {
        disableBackground();
        if(currentlyActiveMenu != null)
        {
            closeMenu(currentlyActiveMenu);
        }
        else
        {
            closeMenu(pauseMenu);
        }

        Cursor.lockState = CursorLockMode.Locked;//lock the cursor
        Time.timeScale = 1;
        gameFrozen = false;
        _playerInput.SwitchCurrentActionMap("Player");
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    }
    #endregion

    #region BackgroundImage
    void enableBackground()
    {
        if (showBackground)
        {
            background.gameObject.SetActive(true);
            background.GetComponent<Image>().enabled = true;
        }
    }

    void disableBackground()
    {
        background.gameObject.SetActive(false);
        background.GetComponent<Image>().enabled = false;
    }
    #endregion

    #region Public Menu Functions
    public void OpenPauseMenu()
    {
        openMenu(pauseMenu);
    }

    //For close pause menu, you probably want to use public function UnFreezeGame() from above instead.

    public void openGameplayMenu()
    {
        openMenu(gameplayMenu);
    }

    public void openGraphicsMenu()
    {
        openMenu(graphicsMenu);
    }

    public void openAudioMenu()
    {
        openMenu(audioMenu);
    }

    #endregion

}


