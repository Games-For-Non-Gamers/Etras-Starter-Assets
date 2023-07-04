using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

using static Etra.StarterAssets.EtraCharacterMainController;

namespace Etra.StarterAssets.Source.Editor
{
    public static partial class EtraGUIUtility
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            _fpImage = Resources.Load<Texture2D>("CharacterCreatorFP");
            _tpImage = Resources.Load<Texture2D>("CharacterCreatorTP");

            _defaultModelImage = Resources.Load<Texture2D>("CharacterCreatorModelNone");
            _modelImages = new Dictionary<Model, Texture2D>()
            {
                [Model.DefaultArmature] = Resources.Load<Texture2D>("CharacterCreatorModelArmature"),
                [Model.Capsule] = Resources.Load<Texture2D>("CharacterCreatorModelCapsule"),
                [Model.Voxel] = Resources.Load<Texture2D>("CharacterCreatorModelVoxel"),
                [Model.None] = Resources.Load<Texture2D>("CharacterCreatorModelNone"),
            };
        }

        #region Generic GUI
        public static Color BorderColor => EditorGUIUtility.isProSkin ? new Color(0.1372549019607843f, 0.1372549019607843f, 0.1372549019607843f) : new Color(0.6f, 0.6f, 0.6f);
        public static Texture2D BorderTexture => GenerateColorTexture(BorderColor);

        public static Texture2D GenerateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static void VerticalLineLayout()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedWidth = 1f,
                stretchHeight = true,
            };

            style.normal.background = BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }

        public static void HorizontalLineLayout()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedHeight = 1f,
                stretchWidth = true,
            };

            style.normal.background = BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }
        #endregion

        #region Models
        static Texture2D _fpImage;
        static Texture2D _tpImage;

        private static Dictionary<Model, string> _modelDescriptions = new Dictionary<Model, string>()
        {
            [Model.DefaultArmature] = "Default humanoid model with animations",
            [Model.Capsule] = "Default Unity capsule without animations",
            [Model.Voxel] = "Stylized voxel model with animations",
            [Model.None] = "No model",
        };

        static Texture2D _defaultModelImage;
        private static Dictionary<Model, Texture2D> _modelImages = new Dictionary<Model, Texture2D>();

        public static void ModelSelectorLayout(Model model, Action<Model> onChangeModel, Model[] avaliableModels) =>
            ModelSelectorLayout("Model", model, onChangeModel, avaliableModels);

        public static void ModelSelectorLayout(string label, Model model, Action<Model> onChangeModel, Model[] avaliableModels)
        {
            if (!avaliableModels.Contains(model))
                model = avaliableModels.FirstOrDefault();

            //GUILayout.Label("FPS Model", Styles.Header);
            GUILayout.Label("Character Model", Styles.Header);

            Rect rect = GUILayoutUtility.GetAspectRect(1f / 0.4f)
                .Border(4f, 0f);

            GUIContent descriptionContent = _modelDescriptions.ContainsKey(model) ?
                new GUIContent(_modelDescriptions[model]) :
                GUIContent.none;

            Rect imageRect = new Rect(rect)
                .SetWidth(rect.height);

            Rect popupRect = new Rect(rect)
                .ResizeToRight(rect.width - rect.height - 4f)
                .SetHeight(32f);

            Rect descriptionRect = new Rect(popupRect)
                .MoveY(36f)
                .SetHeight(EditorStyles.helpBox.CalcHeight(descriptionContent, popupRect.width));

            GUI.Label(imageRect, GUIContent.none, Styles.Button);
            GUI.DrawTexture(imageRect.Border(4f), _modelImages.ContainsKey(model) ? _modelImages[model] : _defaultModelImage);

            //DIY popup, because the built in one didn't want to expand vertically
            if (GUI.Button(popupRect, model.ToString(), Styles.Popup))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var type in avaliableModels)
                    menu.AddItem(new GUIContent(type.ToString()), model == type, () => onChangeModel?.Invoke(type));

                menu.DropDown(popupRect);
            }

            GUI.Label(descriptionRect, descriptionContent, EditorStyles.helpBox);
        }
        #endregion

        #region Gameplay Type
        public static GameplayType GameplayTypeSelectorLayout(GameplayType gameplayType) =>
            GameplayTypeSelectorLayout("Character Type", gameplayType);

        public static GameplayType GameplayTypeSelectorLayout(string label, GameplayType gameplayType)
        {
            GUILayout.Label(label, Styles.Header);

            Rect startRect = GUILayoutUtility.GetAspectRect(2f / 1f)
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


            if (GUI.Toggle(fpRect, gameplayType == GameplayType.FirstPerson, GUIContent.none, Styles.Button))
                gameplayType = GameplayType.FirstPerson;

            if (GUI.Toggle(tpRect, gameplayType == GameplayType.ThirdPerson, GUIContent.none, Styles.Button))
                gameplayType = GameplayType.ThirdPerson;


            GUI.DrawTexture(fpImageRect, _fpImage);
            GUI.DrawTexture(tpImageRect, _tpImage);

            GUI.Label(fpTextRect, "First Person", Styles.GameplayTypeLabel);
            GUI.Label(tpTextRect, "Third Person", Styles.GameplayTypeLabel);

            return gameplayType;
        }
        #endregion

        #region Reflections
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

        public static bool CheckForUsage(Type type, GameplayTypeFlags gameplayType)
        {
            var attribute = type.GetCustomAttribute<AbilityUsageAttribute>();
            if (attribute == null)
                return gameplayType == GameplayTypeFlags.All;

            return attribute.GameplayType.HasFlag(gameplayType) &&
                attribute.GameplayType != GameplayTypeFlags.All;
        }
        #endregion

        public static class Styles
        {
            public static GUIStyle Button => new GUIStyle("Button");

            public static GUIStyle Popup => new GUIStyle(EditorStyles.popup)
            {
                fixedHeight = 32f,
            };

            public static GUIStyle GameplayTypeLabel = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
            };

            public static GUIStyle StandardText = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
            };

            public static GUIStyle Header => new GUIStyle(EditorStyles.whiteLargeLabel)
            {
                fontSize = 18,
            };
        }
    }
}