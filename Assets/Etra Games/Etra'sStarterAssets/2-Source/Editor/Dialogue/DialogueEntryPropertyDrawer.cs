using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace Etra.StarterAssets.Source.Editor
{
    [CustomPropertyDrawer(typeof(DialogueEntry))]
    public class DialogueEntryPropertyDrawer : PropertyDrawer
    {
        //Set block height, sadly I wasn't able to make this variable    
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            DialogueEntry.DialogueEvents state = (DialogueEntry.DialogueEvents)prop.FindPropertyRelative("chosenEvent").enumValueIndex;
            switch (state)
            {
                case DialogueEntry.DialogueEvents.PlayAudioFromManager:
                    return base.GetPropertyHeight(prop, label) + 30;
                case DialogueEntry.DialogueEvents.UpdateLine:
                    return base.GetPropertyHeight(prop, label) + 70;
                case DialogueEntry.DialogueEvents.Wait:
                    return base.GetPropertyHeight(prop, label) + 30;
                default:
                    return base.GetPropertyHeight(prop, label) + 70;
            }
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            // Rect rect1 = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
            EditorGUI.LabelField(new Rect(pos.x, pos.y, 120, 20), "Event:");
            EditorGUI.PropertyField(new Rect(pos.x + 120, pos.y, pos.width - 80, 20), prop.FindPropertyRelative("chosenEvent"), GUIContent.none);

            DialogueEntry.DialogueEvents state = (DialogueEntry.DialogueEvents)prop.FindPropertyRelative("chosenEvent").enumValueIndex;
            switch (state)
            {
                case DialogueEntry.DialogueEvents.PlayAudioFromManager:
                    SerializedProperty sfxName = prop.FindPropertyRelative("sfxName");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y+ 20, pos.width, 20), sfxName);
                    break;

                case DialogueEntry.DialogueEvents.UpdateLine:
                    SerializedProperty speaker = prop.FindPropertyRelative("speaker");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y+20, pos.width, 20), speaker);

                    SerializedProperty dialogueLine = prop.FindPropertyRelative("dialogueLine");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 40, pos.width, 20), dialogueLine);

                    SerializedProperty timeTillNextEvent = prop.FindPropertyRelative("timeTillNextEvent");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 60, pos.width, 20), timeTillNextEvent);
                    break;

                case DialogueEntry.DialogueEvents.Wait:
                    SerializedProperty delay = prop.FindPropertyRelative("timeTillNextEvent");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y+20, pos.width, 20), delay);
                    break;

                default:
                    break;
            }



        }
    }
        
}
