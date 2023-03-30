using StarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

using static StarterAssets.EtraCharacterMainController;
using static EtraFPSUsableItemManager;

using UObject = UnityEngine.Object;
using System.Reflection;

//From Dock Frankenstein
////https://www.youtube.com/channel/UCq_7pbSyOvrurXLAMi_Ss1w
/*
The MIT License (MIT)
Copyright 2023 Dock Frankenstein
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

public class EtraCharacterCreator : EditorWindow, IHasCustomMenu
{
    const int PAGE_LIMIT = 4;
    const string PAGE_SESSION_KEY = "etra_character_creator_page";
    const float DEFAULT_WINDOW_WIDTH = 400f;
    const float DEFAULT_WINDOW_HEIGHT = 600f;

    int? _page = null;
    int Page
    {
        get
        {
            if (_page == null)
                _page = SessionState.GetInt(PAGE_SESSION_KEY, 0);
            return _page ?? 0;
        }
        set
        {
            _page = value;
            SessionState.SetInt(PAGE_SESSION_KEY, value); 
        }
    }

    GameplayType _gameplayType = GameplayType.FirstPerson;
    Model _model = Model.DefaultArmature;
    Vector2 _scroll;

    [NonSerialized] bool _init = false;


    List<Ability> generalAbilities = new List<Ability>();
    List<Ability> fpAbilities = new List<Ability>();
    List<Ability> tpAbilities = new List<Ability>();
    List<Ability> fpsItems = new List<Ability>();

    EtraCharacterMainController _target;

    GUIStyle s_barSpace;
    GUIStyle s_title;
    GUIStyle s_descriptionBackground;
    GUIStyle s_wrappedLabel;
    GUIStyle s_header;
    GUIStyle s_modelPopup;

    Texture2D fpImage;
    Texture2D tpImage;

    private static Dictionary<Model, string> _modelDescriptions = new Dictionary<Model, string>()
    {
        [Model.DefaultArmature] = "Default humanoid model with animations",
        [Model.Capsule] = "Default Unity capsule without animations",
        [Model.Voxel] = "Stylized voxel model with animations",
        [Model.None] = "No model",
    };

    Texture2D _defaultModelImage;
    private static Dictionary<Model, Texture2D> _modelImages = new Dictionary<Model, Texture2D>();

    Rect r_modelPopup;


    #region Inherited
    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Prefs/Keep Opened"), Preferences.KeepOpened, () => Preferences.KeepOpened = !Preferences.KeepOpened);
        menu.AddItem(new GUIContent("Prefs/Log"), Preferences.Log, () => Preferences.Log = !Preferences.Log);
    }

    private void OnHierarchyChange()
    {
        RefreshTarget();
        Repaint();
    }
    #endregion

    #region Generic
    [MenuItem("Window/Etra's Starter Assets/Etra Character Creator")]
    public static void ShowWindow()
    {
        //Set Window size and name
        EtraCharacterCreator window = GetWindow<EtraCharacterCreator>();
        window.titleContent = new GUIContent("Character Creator");

        if (!Preferences.FirstTime)
        {
            DebugLog("Window oppened for the first time");
            Preferences.FirstTime = true;
            window.minSize = new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
            window.maxSize = new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
        }

        window.minSize = new Vector2(400f, 400f);
        window.maxSize = new Vector2(800f, 800f);

        window.Page = 0;

        window.RefreshTarget();
        window.GenerateAbilitiesAndItems();
        window.RemoveStates();
        window.CreateStatesFromPlayer();

        window.Show();

        DebugLog("Window Opened");
    }

    public void CloseWindow()
    {
        RemoveStates();
        Close();
    }

    void Initialize()
    {
        //Styles
        s_barSpace = new GUIStyle()
        {
            margin = new RectOffset(4, 4, 4, 4)
        };

        s_title = new GUIStyle(EditorStyles.label)
        {
            fontSize = 32,
            fontStyle = FontStyle.Bold,
            margin = new RectOffset(4, 4, 4, 0),
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 120f,
        };

        s_title.normal.background = Resources.Load<Texture2D>("CharacterCreatorTitleBackground");
        s_title.normal.textColor = Color.white;

        s_descriptionBackground = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(8, 8, 8, 8),
        };

        s_wrappedLabel = new GUIStyle(EditorStyles.label)
        {
            wordWrap = true,
            fontSize = 13,
        };

        s_header = new GUIStyle(EditorStyles.whiteLargeLabel)
        {
            fontSize = 18,
        };

        s_modelPopup = new GUIStyle(EditorStyles.popup)
        {
            fixedHeight = 32f,
        };

        //Images
        fpImage = Resources.Load<Texture2D>("CharacterCreatorFP");
        tpImage = Resources.Load<Texture2D>("CharacterCreatorTP");

        _defaultModelImage = Resources.Load<Texture2D>("CharacterCreatorModelNone");
        _modelImages = new Dictionary<Model, Texture2D>()
        {
            [Model.DefaultArmature] = Resources.Load<Texture2D>("CharacterCreatorModelArmature"),
            [Model.Capsule] = Resources.Load<Texture2D>("CharacterCreatorModelCapsule"),
            [Model.Voxel] =  Resources.Load<Texture2D>("CharacterCreatorModelVoxel"),
            [Model.None] = Resources.Load<Texture2D>("CharacterCreatorModelNone"),
        };

        //Get target
        RefreshTarget();

        GenerateAbilitiesAndItems();
        LoadStates();

        _init = true;
        DebugLog("Initialized");
    }
    #endregion

    #region GUI
    private void OnGUI()
    {
        if (!_init)
            Initialize();

        using (var scope = new GUILayout.ScrollViewScope(_scroll))
        {
            using (var change = new EditorGUI.ChangeCheckScope())
            { 
                ContentGUI();

                if (change.changed)
                    SaveStates();
            }

            GUILayout.FlexibleSpace();

            _scroll = scope.scrollPosition;
        }

        EtraGUIUtility.HorizontalLineLayout();

        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"Page {Page + 1}/{PAGE_LIMIT}", GUILayout.Width(60f));
                BarGUI();
            }

            //Skip buttons
            using (new GUILayout.VerticalScope(GUILayout.Height(26f)))
                GUILayout.FlexibleSpace();

            Rect spaceRect = GUILayoutUtility.GetLastRect()
                .Border(2f, 2f, 0f, 2f);
            Rect previousButtonRect = new Rect(spaceRect)
                .ResizeToLeft(spaceRect.width / 2f - 2f);
            Rect nextButtonRect = new Rect(spaceRect)
                .ResizeToRight(spaceRect.width / 2f - 2f);

            using (new EditorGUI.DisabledScope(Page <= 0))
                if (GUI.Button(previousButtonRect, "<< Previous"))
                    SkipPage(-1);

            switch (Page + 1 < PAGE_LIMIT)
            {
                case true:
                    if (GUI.Button(nextButtonRect, "Next >>"))
                        SkipPage(1);
                    break;
                case false:
                    using (new EditorGUI.DisabledScope(Application.isPlaying))
                        if (GUI.Button(nextButtonRect, _target == null ? "Create" : "Modify"))
                            CreateOrModify();
                    break;
            }
        }
    }

    void BarGUI()
    {
        GUILayout.BeginVertical(s_barSpace, GUILayout.Height(14f));
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        Rect barRect = GUILayoutUtility.GetLastRect();

        EditorGUI.ProgressBar(barRect, (float)Page / (PAGE_LIMIT - 1), string.Empty);
    }

    //Variables for page 2 model swapping
    bool characterTypeSelectionChanged = true;
    GameplayType savedType;

    void ContentGUI()
    {
        switch (Page)
        {
            case 0: //"Etra's Character Creator" PAGE
                GUILayout.Label("Etra's Character Creator", s_title);

                int linkIndex = GUILayout.SelectionGrid(-1, new string[] { "Documentation", "Discord", "Tutorials" }, 3);

                //This will definetely not work on Unity 2019
                if (linkIndex != -1)
                    Application.OpenURL(linkIndex switch
                    {
                        0 => "Assets\\Etra Games\\Etra'sStarterAssets\\1-UserAssets\\Etra'sStarterAssets_Documentation.pdf",
                        1 => "https://discord.gg/d3AzQDGj4C",
                        2 => "https://www.youtube.com/playlist?list=PLvmCfejZtwhO7w1sI0DAMHWqrr6JMABpD",
                        _ => string.Empty,
                    });

                EditorGUILayout.Space(2f);

                using (new GUILayout.VerticalScope(s_descriptionBackground))
                    GUILayout.Label("Welcome to the Etra's Starter Assets: Character Creator! \n\nThis setup wizard will allow you to create and modify the character controller, along with its different abilities. \n\nEvery setting is dynamically generated, so your own abilities and items will also show up here. \n\nIf you feel stuck at any point, you can ask for help on our discord server (link above).", s_wrappedLabel);
                
                break;
            case 1: //"Character Type" PAGE
                GUIStyle buttonStyle = new GUIStyle("Button");
                GUIStyle labelStyle = new GUIStyle(EditorStyles.miniBoldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                };

                GUILayout.Label("Character Type", s_header);

                Rect startRect = GUILayoutUtility.GetAspectRect(2f/1f)
                    .Border(4f);

                GUILayout.Space(8f);

                startRect = startRect
                    .SetHeight(startRect.width * 0.5f - 2f)
                    .BorderBottom(-14f);

                Rect fpRect = startRect
                    .ResizeToLeft(startRect.width * 0.5f)
                    .BorderRight(2f);

                Rect tpRect = startRect
                    .ResizeToRight(startRect.width * 0.5f)
                    .BorderLeft(2f);

                Rect fpTextRect = fpRect
                    .ResizeToBottom(18f)
                    .MoveY(-2f);

                Rect tpTextRect = tpRect
                    .ResizeToBottom(18f)
                    .MoveY(-2f);

                Rect fpImageRect = fpRect
                    .ResizeToTop(fpRect.width)
                    .Border(8f);

                Rect tpImageRect = tpRect
                    .ResizeToTop(tpRect.width)
                    .Border(8f);



                
                if (savedType!=_gameplayType)
                {
                    characterTypeSelectionChanged = true;
                }
                savedType = _gameplayType;

                if (GUI.Toggle(fpRect, _gameplayType == GameplayType.FirstPerson, GUIContent.none, buttonStyle))
                {
                    _gameplayType = GameplayType.FirstPerson;
                    if (characterTypeSelectionChanged)
                    {
                        _model = Model.Capsule;
                        characterTypeSelectionChanged = false;
                    }
                }
                    

                if (GUI.Toggle(tpRect, _gameplayType == GameplayType.ThirdPerson, GUIContent.none, buttonStyle))
                {
                    _gameplayType = GameplayType.ThirdPerson;

                    if (characterTypeSelectionChanged)
                    {
                        _model = Model.DefaultArmature;
                        characterTypeSelectionChanged = false;
                    }
                }

                



                GUI.DrawTexture(fpImageRect, fpImage);
                GUI.DrawTexture(tpImageRect, tpImage);

                GUI.Label(fpTextRect, "First Person", labelStyle);
                GUI.Label(tpTextRect, "Third Person", labelStyle);

                if (_gameplayType == GameplayType.FirstPerson)
                {
                    EditorGUILayout.Space(8f);
                    GUILayout.Label("FPS Model", s_header);

                    using (new GUILayout.HorizontalScope(GUILayout.MinHeight(position.width * 0.4f)))
                    {
                        GUILayout.Space(4f);

                        GUILayout.Space(position.width * 0.4f);

                        Rect imageRect = GUILayoutUtility.GetLastRect();
                        imageRect = imageRect
                            .SetHeight(imageRect.width);

                        GUI.Label(imageRect, GUIContent.none, buttonStyle);
                        GUI.DrawTexture(imageRect.Border(4f), _modelImages.ContainsKey(_model) ? _modelImages[_model] : _defaultModelImage);

                        GUILayout.Space(4f);

                        using (new GUILayout.VerticalScope())
                        {
                            //DIY popup, because the built in one didn't want to expand vertically
                            if (GUILayout.Button(_model.ToString(), s_modelPopup))
                            {
                                GenericMenu menu = new GenericMenu();
                                foreach (var type in Enum.GetValues(typeof(Model)))
                                {
                                    if ((Model)type == Model.None || (Model)type == Model.Capsule)
                                    {
                                        menu.AddItem(new GUIContent(type.ToString()), _model == (Model)type, () => _model = (Model)type);
                                    }
                                }

                                menu.DropDown(r_modelPopup);

                            }

                            if (Event.current.type == EventType.Repaint)
                                r_modelPopup = GUILayoutUtility.GetLastRect();

                            GUILayout.Label(_modelDescriptions.ContainsKey(_model) ? _modelDescriptions[_model] : string.Empty, EditorStyles.helpBox);
                        }
                    }
                }



                if (_gameplayType == GameplayType.ThirdPerson)
                {
                    EditorGUILayout.Space(8f);
                    GUILayout.Label("TPS Model", s_header);

                    using (new GUILayout.HorizontalScope(GUILayout.MinHeight(position.width * 0.4f)))
                    {
                        GUILayout.Space(4f);

                        GUILayout.Space(position.width * 0.4f);

                        Rect imageRect = GUILayoutUtility.GetLastRect();
                        imageRect = imageRect
                            .SetHeight(imageRect.width);

                        GUI.Label(imageRect, GUIContent.none, buttonStyle);
                        GUI.DrawTexture(imageRect.Border(4f), _modelImages.ContainsKey(_model) ? _modelImages[_model] : _defaultModelImage);

                        GUILayout.Space(4f);

                        using (new GUILayout.VerticalScope())
                        {
                            //DIY popup, because the built in one didn't want to expand vertically
                            if (GUILayout.Button(_model.ToString(), s_modelPopup))
                            {
                                GenericMenu menu = new GenericMenu();
                                foreach (var type in Enum.GetValues(typeof(Model)))
                                {
                                    menu.AddItem(new GUIContent(type.ToString()), _model == (Model)type, () => _model = (Model)type);
                                }

                                menu.DropDown(r_modelPopup);

                            }

                            if (Event.current.type == EventType.Repaint)
                                r_modelPopup = GUILayoutUtility.GetLastRect();

                            GUILayout.Label(_modelDescriptions.ContainsKey(_model) ? _modelDescriptions[_model] : string.Empty, EditorStyles.helpBox);
                        }
                    }
                }

                break;
            case 2: //"General Abilities" PAGE
                GUILayout.Label("General Abilities", s_header);
                foreach (var item in generalAbilities)
                    item.AbilityGUI();
                break;
            case 3: //"Genre Specific Abilities" PAGE
                string header = _gameplayType switch
                {
                    GameplayType.FirstPerson => "First Person Abilities",
                    GameplayType.ThirdPerson => "Third Person Abilities",
                    _ => "Abilities",
                };

                var specificAbilities = _gameplayType switch
                {
                    GameplayType.FirstPerson => fpAbilities,
                    GameplayType.ThirdPerson => tpAbilities,
                    _ => new List<Ability>(),
                };

                GUILayout.Label(header, s_header);
                foreach (var item in specificAbilities)
                    item.AbilityGUI();

                if (_gameplayType == GameplayType.FirstPerson)
                {
                    EditorGUILayout.Space();

                    GUILayout.Label("First Person Items", s_header);
                    foreach (var item in fpsItems)
                        item.AbilityGUI();
                }

                break;
        }
    }
    #endregion

    #region State
    const string _WINDOW_STATE_KEY_PREFIX = "etra_character_creator_";
    const string _WINDOW_STATE_GAMEPLAY_TYPE_KEY = "etra_character_creator_gameplay_type";
    const string _WINDOW_STATE_MODEL_KEY = "etra_character_creator_model";

    /// <summary>Creates session states from the target</summary>
    /// <param name="loadStates">Should the data be loaded?</param>
    void CreateStatesFromPlayer(bool loadStates = true)
    {
        if (_target == null)
            return;

        SessionState.SetInt(_WINDOW_STATE_GAMEPLAY_TYPE_KEY, (int)_target.appliedGameplayType);
        SessionState.SetInt(_WINDOW_STATE_MODEL_KEY, (int)(
            _target.appliedGameplayType == GameplayType.ThirdPerson ? _target.appliedCharacterModel :  Model.DefaultArmature));

        //Abilities
        var addedAbilities = _target.abilityManager.characterAbilityUpdateOrder
            .Where(x => x != null)
            .Select(x => x.GetType())
            .ToList();

        foreach (var item in generalAbilities)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", addedAbilities.Contains(item.type));

        foreach (var item in fpAbilities)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", addedAbilities.Contains(item.type));

        foreach (var item in tpAbilities)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", addedAbilities.Contains(item.type));

        //Items
        var addedItems = FindObjectOfType<EtraFPSUsableItemManager>()?.usableItems
            .Where(x => x?.script != null)
            .Select(x => x.script.GetType())
            .ToList() ?? new List<Type>();

        foreach (var item in fpsItems)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", addedItems.Contains(item.type));

        if (loadStates)
            LoadStates();
    }

    /// <summary>Loads window data from session state</summary>
    void LoadStates()
    {
        _gameplayType = (GameplayType)SessionState.GetInt(_WINDOW_STATE_GAMEPLAY_TYPE_KEY, 0);
        _model = (Model)SessionState.GetInt(_WINDOW_STATE_MODEL_KEY, 0);

        foreach (var item in generalAbilities)
            item.state = SessionState.GetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", false);

        foreach (var item in fpAbilities)
            item.state = SessionState.GetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", false);

        foreach (var item in tpAbilities)
            item.state = SessionState.GetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", false);

        foreach (var item in fpsItems)
            item.state = SessionState.GetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", false);
    }

    void SaveStates() 
    {
        SessionState.SetInt(_WINDOW_STATE_GAMEPLAY_TYPE_KEY, (int)_gameplayType);
        SessionState.SetInt(_WINDOW_STATE_MODEL_KEY, (int)_model);

        foreach (var item in generalAbilities)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", item.state);

        foreach (var item in fpAbilities)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", item.state);

        foreach (var item in tpAbilities)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", item.state);

        foreach (var item in fpsItems)
            SessionState.SetBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}", item.state);
    }

    /// <summary>Clears out window data from session state</summary>
    void RemoveStates()
    {
        SessionState.EraseInt(_WINDOW_STATE_GAMEPLAY_TYPE_KEY);
        SessionState.EraseInt(_WINDOW_STATE_MODEL_KEY);

        foreach (var item in generalAbilities)
            SessionState.EraseBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}");

        foreach (var item in fpAbilities)
            SessionState.EraseBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}");

        foreach (var item in tpAbilities)
            SessionState.EraseBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}");

        foreach (var item in fpsItems)
            SessionState.EraseBool($"{_WINDOW_STATE_KEY_PREFIX}{item.type.FullName}");
    }
    #endregion

    #region Actions
    void GenerateAbilitiesAndItems()
    {
        //Initialize abilities
        generalAbilities = FindAllTypes<EtraAbilityBaseClass>()
            .Select(x => new Ability(x))
            .ToList();

        fpAbilities = generalAbilities
            .Where(x => CheckForUsage(x.type, GameplayTypeFlags.FirstPerson))
            .ToList();

        tpAbilities = generalAbilities
            .Where(x => CheckForUsage(x.type, GameplayTypeFlags.ThirdPerson))
            .ToList();

        generalAbilities = generalAbilities
            .Except(fpAbilities)
            .Except(tpAbilities)
            .ToList();

        //Initialize items
        fpsItems = FindAllTypes<EtraFPSUsableItemBaseClass>()
            .Select(x => new Ability(x))
            .ToList();
    }

    void RefreshTarget()
    {
        _target = FindObjectOfType<EtraCharacterMainController>();
    }

    void SkipPage(int amount)
    {
        Page += amount;
        Page = Mathf.Clamp(Page, 0, PAGE_LIMIT);
    }

    public void CreateOrModify()
    {
        GameObject group = _target == null ?
            EtrasResourceGrabbingFunctions.addPrefabFromResourcesByName("EtraCharacterAssetGroup") :
            GetRootParent(_target.transform).gameObject;

        if (_target == null)
            _target = group.GetComponentInChildren<EtraCharacterMainController>();

        var abilityManager = _target.abilityManager;

        var selectAbilityScriptTypes = generalAbilities
            .Concat(_gameplayType switch
            {
                GameplayType.FirstPerson => fpAbilities,
                GameplayType.ThirdPerson => tpAbilities,
                _ => new List<Ability>(),
            })
            .Select(x => x.type)
            .ToList();

        foreach (var item in abilityManager.characterAbilityUpdateOrder)
            if (item != null && !selectAbilityScriptTypes.Contains(item.GetType()))
                DestroyImmediate(item, true);

        abilityManager.characterAbilityUpdateOrder = new EtraAbilityBaseClass[0];

        //Add base abilities
        AddAbilities(abilityManager, generalAbilities, false);

        switch (_gameplayType)
        {
            case GameplayType.FirstPerson:
                AddAbilities(abilityManager, fpAbilities, log: "Adding first person ability");

                //Add FPS Items
                bool requiresItems = fpsItems
                    .Where(x => x.state)
                    .Count() > 0;

                switch (requiresItems)
                {
                    case true:
                        //TODO: don't
                        var itemManager = FindObjectOfType<EtraFPSUsableItemManager>() ??
                            EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("EtraFPSUsableItemManagerPrefab", _target.transform)
                            .GetComponent<EtraFPSUsableItemManager>();

                        var itemsToDelete = itemManager.usableItems;

                        foreach (var item in itemsToDelete)
                            DestroyImmediate(item.script, true);

                        itemManager.Reset();

                        foreach (var item in fpsItems)
                        {
                            if (!item.state) continue;
                            var component = (EtraFPSUsableItemBaseClass)itemManager.gameObject.AddComponent(item.type);
                            itemManager.usableItems = itemManager.usableItems
                                .Concat(new usableItemScriptAndPrefab[] { new usableItemScriptAndPrefab(component) })
                                .ToArray();

                            DebugLog($"Adding item '{item.name}'");
                        }

                        break;
                    case false:
                        var manager = FindObjectOfType<EtraFPSUsableItemManager>();

                        if (manager != null)
                            DestroyImmediate(manager.gameObject, true);
                        break;
                }

                var aimCamera = GameObject.Find("Etra'sStarterAssetsThirdPersonAimCamera");
                if (aimCamera != null)
                    DestroyImmediate(aimCamera.gameObject, true);



                _target.applyGameplayChanges(_gameplayType, _model);
                break;

            case GameplayType.ThirdPerson:
                var usableItemManager = FindObjectOfType<EtraFPSUsableItemManager>();
                if (usableItemManager != null)
                    DestroyImmediate(usableItemManager.gameObject, true);

                var itemCamera = GameObject.Find("FPSUsableItemsCamera");
                if (itemCamera != null)
                    DestroyImmediate(itemCamera.gameObject, true);


                AddAbilities(abilityManager, tpAbilities, log: "Adding third person ability");
                _target.applyGameplayChanges(_gameplayType, _model);
                break;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.objects = new UObject[] { group };

        if (!Preferences.KeepOpened)
            CloseWindow();
    }


    #endregion

    #region Utility
    public static List<Type> FindAllTypesList<T>() =>
        FindAllTypes<T>()
            .ToList();

    public static IEnumerable<Type> FindAllTypes<T>()
    {
        var type = typeof(T);
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => t != type && type.IsAssignableFrom(t));
    }

    static void DebugLog(string text)
    {
        if (Preferences.Log)
            Debug.Log($"[Etra Character Creator] {text}");
    }

    static bool CheckForUsage(Type type, GameplayTypeFlags gameplayType)
    {
        var attribute = type.GetCustomAttribute<AbilityUsage>();
        if (attribute == null)
            return gameplayType == GameplayTypeFlags.All;

        return attribute.GameplayType.HasFlag(gameplayType) &&
            attribute.GameplayType != GameplayTypeFlags.All;
    }

    void AddAbilities(EtraAbilityManager abilityManager, List<Ability> abilities, bool assignScripts = true, string log = "Adding generic ability")
    {
        foreach (var item in abilities)
        {
            if (!item.state || abilityManager.gameObject.GetComponent(item.type)) continue;

            abilityManager.gameObject.AddComponent(item.type);

            DebugLog($"{log} '{item.name}'");
        }

        if (assignScripts)
            abilityManager.characterAbilityUpdateOrder = abilityManager.gameObject.GetComponents<EtraAbilityBaseClass>();
    }

    Transform GetRootParent(Transform trans)
    {
        if (trans == null)
            return trans;

        while (true)
        {
            if (trans.parent == null)
                return trans;

            trans = trans.parent;
        }
    }
    #endregion

    class Ability
    {
        public Ability(Type type)
        {
            this.type = type;
            state = false;
            GenerateName();
        }

        public Type type;
        public string name;
        public bool state;

        public void GenerateName()
        {
            name = type.Name
                .Split('_')
                .Last();

            name = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        }

        public void AbilityGUI()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label(name);

                DrawToggleButton(false);
                DrawToggleButton(true);
            }
        }

        /// <param name="type">If <see langword="true"/>, the button will display "On"; If <see langword="false"/>, the button will display "Off"</param>
        void DrawToggleButton(bool type)
        {
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                var color = GUI.backgroundColor;
                if (state == type)
                    GUI.backgroundColor = type ? Color.green : Color.red;

                bool toggle = GUILayout.Toggle(state == type, 
                    type ? "On" : "Off", 
                    type ? EditorStyles.miniButtonRight : EditorStyles.miniButtonLeft, 
                    GUILayout.Width(40f));

                GUI.backgroundColor = color;

                if (change.changed && toggle)
                    state = type;
            }
        }
    }

    private static class Preferences
    {
        private const string _KEEP_OPENED_KEY = "etra_character_creator_keep_opened";
        private static bool? _keepOpened = null;
        public static bool KeepOpened
        {
            get
            {
                if (_keepOpened == null)
                    _keepOpened = EditorPrefs.GetBool(_KEEP_OPENED_KEY, false);

                return _keepOpened ?? false;
            }
            set
            {
                _keepOpened = value;
                EditorPrefs.SetBool(_KEEP_OPENED_KEY, value);
            }
        }

        private const string _LOG_KEY = "etra_character_creator_log";
        private static bool? _log = null;
        public static bool Log
        {
            get
            {
                if (_log == null)
                    _log = EditorPrefs.GetBool(_LOG_KEY, false);

                return _log ?? false;
            }
            set
            {
                _log = value;
                EditorPrefs.SetBool(_LOG_KEY, value);
            }
        }

        private const string _FIRST_TIME = "etra_character_creator_first";
        private static bool? _firstTime = null;
        public static bool FirstTime
        {
            get
            {
                if (_firstTime == null)
                    _firstTime = EditorPrefs.GetBool(_FIRST_TIME, false);

                return _firstTime ?? false;
            }
            set
            {
                _firstTime = value;
                EditorPrefs.SetBool(_FIRST_TIME, value);
            }
        }
    }
}
