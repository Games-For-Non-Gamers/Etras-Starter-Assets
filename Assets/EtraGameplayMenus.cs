using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


public class EtraGameplayMenus : MonoBehaviour
{

    [Header("Options")]
    public bool canPause = true;

    [Header("References")]
    public GameObject background;
    public EtraGameplayMenu pauseMenu;
    public EtraGameplayMenu gameplayMenu;
    public EtraGameplayMenu graphicsMenu;
    public EtraGameplayMenu audioMenu;

    bool gamePaused = false;
    EtraGameplayMenu currentlyActiveMenu;

    //private references
    EventSystem eventSystem;
    public InputSystemUIInputModule inputModule;

    #region PauseInput
    //************** INPUTS/CONTROLLER OR KEYBOARD SELECTION FOR PAUSE EVENT************** 
    StarterAssetsInputs _inputs;
    public PlayerInput playerInput;
    private void Start()
    {
        _inputs = FindObjectOfType<StarterAssetsInputs>();
        eventSystem = GetComponentInChildren<EventSystem>();

        if (EtraCharacterMainController.Instance)
        {
            EtraCharacterMainController.Instance.disableAllActiveAbilities();
        }
        unpauseGame();
        disableBackground();
    }


    private InputAction keyboardEscape;
    private InputAction gamepadStart;
    void OnEnable()
    {
        // Enable the key action
        keyboardEscape = new InputAction("KeyboardEnter", binding: "<Keyboard>/enter");
        keyboardEscape.Enable();
        gamepadStart = new InputAction("GamepadStart", binding: "<Gamepad>/start");
        gamepadStart.Enable();
    }

    void OnDisable()
    {
        keyboardEscape.Disable();
        keyboardEscape.Dispose();
        gamepadStart.Disable();
        gamepadStart.Dispose();
    }


    bool isUsingKeyboard;
    bool isUsingGamepad;
    void Update()
    {
        if (canPause)
        {
            if (keyboardEscape.triggered || gamepadStart.triggered)
            {
                eventSystem.SetSelectedGameObject(null);
                pauseOrUnpause();
            }

        }

        //If the game is paused check if selection should be with mouse/keyboard or gamepad
        if (gamePaused)
        {
            checkInputDevice();
        }
    }

    GameObject savedSelectedObject = null;
    private void checkInputDevice()
    {
        if (Keyboard.current != null && (Keyboard.current.anyKey.isPressed || Mouse.current.delta.ReadValue().magnitude > 0 || Mouse.current.leftButton.isPressed))
        {
            if (!isUsingKeyboard)
            {
                isUsingKeyboard = true;
                isUsingGamepad = false;
                savedSelectedObject = eventSystem.currentSelectedGameObject;
                eventSystem.SetSelectedGameObject(null);
                Cursor.visible = true;
            }
        }
        else if (Gamepad.current != null && IsAnyGamepadButtonPressed())
        {
            if (!isUsingGamepad)
            {
                isUsingGamepad = true;
                isUsingKeyboard = false;
                Cursor.visible = false;
                if (savedSelectedObject != null && savedSelectedObject.gameObject.activeInHierarchy)
                {
                    eventSystem.SetSelectedGameObject(savedSelectedObject);
                }
                else if (currentlyActiveMenu != null)
                {
                    eventSystem.SetSelectedGameObject(currentlyActiveMenu.firstSelectedObject.gameObject);
                }

            }
        }
    }

    private bool IsAnyGamepadButtonPressed()
    {
        foreach (var button in Gamepad.current.allControls)
        {
            if (button is ButtonControl buttonControl && buttonControl.isPressed)
            {
                return true;
            }
        }
        return false;
    }

    //************************************************************* 
    #endregion

    #region BackgroundImage
    void enableBackground()
    {
        background.gameObject.SetActive(true);
        background.GetComponent<Image>().enabled = true;
    }

    void disableBackground()
    {
        background.gameObject.SetActive(false);
        background.GetComponent<Image>().enabled = false;
    }
    #endregion

    #region MenuFunctions
    void openMenu(EtraGameplayMenu menu)
    {
        menu.gameObject.SetActive(true);
        currentlyActiveMenu = menu;
        checkInputDevice();
        //If we are using gamepad select the new higlighted button
        if (isUsingGamepad)
        {
            eventSystem.SetSelectedGameObject(menu.firstSelectedObject.gameObject);
            Cursor.visible = false;
        }
        else if (isUsingKeyboard)
        {
            eventSystem.SetSelectedGameObject(null);
            Cursor.visible = true;
        }
    }

    void closeMenu(EtraGameplayMenu menu)
    {
        menu.gameObject.SetActive(false);
    }

    #endregion

    #region PauseMenu
    void pauseOrUnpause()
    {
        if (!canPause)
        {
            return;
        }

        if (!gamePaused)
        {
            pauseGame();
        } 
        else if (gamePaused)
        {
            unpauseGame();
        }
    }

    void pauseGame()
    {
        
        enableBackground();
        openMenu(pauseMenu);

        _inputs.SetCursorState(false); //Free the cursor

        if (EtraCharacterMainController.Instance)
        {
            EtraCharacterMainController.Instance.disableAllActiveAbilities();
        }
        Time.timeScale = 0;
        gamePaused = true;
        playerInput.SwitchCurrentActionMap("UI");
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    void unpauseGame()
    {
        disableBackground();
        closeMenu(pauseMenu);
        _inputs.SetCursorState(true); //lock the cursor
        Time.timeScale = 1;
        if (EtraCharacterMainController.Instance)
        {
            EtraCharacterMainController.Instance.enableAllActiveAbilities();
        }
        gamePaused = false;

        playerInput.SwitchCurrentActionMap("Player");
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    }
    #endregion


    #region GameplayMenu
    public void openGameplayMenu()
    {
        //e
    }

    public void closeGameplayMenu()
    {
        //e
    }

    #endregion

}


