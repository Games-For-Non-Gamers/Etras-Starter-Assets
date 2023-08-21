using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Etra.StarterAssets.Source.Editor
{
    [CustomPropertyDrawer(typeof(GameFreezeEntry))]
    public class GameFreezeEntryPropertyDrawer : PropertyDrawer
    {
        //Set block height, sadly I wasn't able to make this variable    
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            GameFreezeEntry.GameFreezeEvents state = (GameFreezeEntry.GameFreezeEvents)prop.FindPropertyRelative("chosenEvent").enumValueIndex;
            switch (state)
            {
                case GameFreezeEntry.GameFreezeEvents.Freeze:
                case GameFreezeEntry.GameFreezeEvents.Unfreeze:
                    return base.GetPropertyHeight(prop, label) + 30;
                case GameFreezeEntry.GameFreezeEvents.Popup:
                    float additionalHeightFromArraySizePopup = 0;

                    SerializedProperty inputsNeededToAdvance = prop.FindPropertyRelative("inputsNeededToAdvance");
                    if (inputsNeededToAdvance.isArray)
                    {
                        additionalHeightFromArraySizePopup = inputsNeededToAdvance.arraySize * 20;
                    }

                    return base.GetPropertyHeight(prop, label) + 230 + additionalHeightFromArraySizePopup;

                case GameFreezeEntry.GameFreezeEvents.AdditionalSfx:
                    float additionalHeightFromArraySizeSfx = 0;

                    SerializedProperty sfxToPlay = prop.FindPropertyRelative("sfxToPlay");
                    if (sfxToPlay.isArray)
                    {
                        additionalHeightFromArraySizeSfx = sfxToPlay.arraySize * 20;
                    }
                    return base.GetPropertyHeight(prop, label) + 60 + additionalHeightFromArraySizeSfx;

                case GameFreezeEntry.GameFreezeEvents.WaitForTime:
                    return base.GetPropertyHeight(prop, label) + 30;

                case GameFreezeEntry.GameFreezeEvents.WaitForInput:

                    float additionalHeightFromArraySizeInput = 0;

                    SerializedProperty inputsNeededToAdvance2 = prop.FindPropertyRelative("inputsNeededToAdvance");
                    if (inputsNeededToAdvance2.isArray)
                    {
                        additionalHeightFromArraySizeInput = inputsNeededToAdvance2.arraySize * 20;
                    }

                    return base.GetPropertyHeight(prop, label) + 60 + additionalHeightFromArraySizeInput;
                default:
                    return base.GetPropertyHeight(prop, label) + 10;
            }
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            // Rect rect1 = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
            EditorGUI.LabelField(new Rect(pos.x, pos.y, 120, 20), "Event:");
            EditorGUI.PropertyField(new Rect(pos.x + 120, pos.y, pos.width - 80, 20), prop.FindPropertyRelative("chosenEvent"), GUIContent.none);

            GameFreezeEntry.GameFreezeEvents state = (GameFreezeEntry.GameFreezeEvents)prop.FindPropertyRelative("chosenEvent").enumValueIndex;
            switch (state)
            {
                case GameFreezeEntry.GameFreezeEvents.Freeze:
                case GameFreezeEntry.GameFreezeEvents.Unfreeze:
                    SerializedProperty backgroundFadeTime = prop.FindPropertyRelative("backgroundFadeTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), backgroundFadeTime);
                    break;

                case GameFreezeEntry.GameFreezeEvents.Popup:
                    SerializedProperty popupToAdd = prop.FindPropertyRelative("popupToAdd");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), popupToAdd);

                    SerializedProperty position = prop.FindPropertyRelative("position");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 40, pos.width, 20), position);

                    SerializedProperty popupText = prop.FindPropertyRelative("popupText");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 60, pos.width, 20), popupText);

                    SerializedProperty popupTextSize = prop.FindPropertyRelative("popupTextSize");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 80, pos.width, 20), popupTextSize);

                    SerializedProperty playDefaultAudio = prop.FindPropertyRelative("playDefaultAudio");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 100, pos.width, 20), playDefaultAudio);

                    SerializedProperty playDefaultAnimation = prop.FindPropertyRelative("playDefaultAnimation");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 120, pos.width, 20), playDefaultAnimation);

                    SerializedProperty advanceType = prop.FindPropertyRelative("advanceType");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 140, pos.width, 20), advanceType);

                    SerializedProperty timeToWait = prop.FindPropertyRelative("timeToWait");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 160, pos.width, 20), timeToWait);

                    SerializedProperty inputsNeededToAdvance = prop.FindPropertyRelative("inputsNeededToAdvance");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 180, pos.width, 20), inputsNeededToAdvance);

                    break;

                case GameFreezeEntry.GameFreezeEvents.AdditionalSfx:
                    SerializedProperty sfxToPlay = prop.FindPropertyRelative("sfxToPlay");  //Need to dynamic size?
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), sfxToPlay);
                    break;

                case GameFreezeEntry.GameFreezeEvents.WaitForTime:
                    SerializedProperty timeToWait2 = prop.FindPropertyRelative("timeToWait");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), timeToWait2);
                    break;

                case GameFreezeEntry.GameFreezeEvents.WaitForInput:
                    SerializedProperty inputsNeededToAdvance2 = prop.FindPropertyRelative("inputsNeededToAdvance");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), inputsNeededToAdvance2); //Need to dynamic size?
                    break;

                default:
                    break;
            }
        }
    }

}
