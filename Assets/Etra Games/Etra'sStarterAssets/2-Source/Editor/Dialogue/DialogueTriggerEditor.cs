using Etra.StarterAssets;
using UnityEditor;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty dialogueList = serializedObject.FindProperty("dialogueList");
        EditorGUILayout.PropertyField(dialogueList);
        serializedObject.ApplyModifiedProperties();
    }

}

