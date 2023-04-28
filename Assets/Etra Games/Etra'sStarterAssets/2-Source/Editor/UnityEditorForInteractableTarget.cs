using UnityEditor;
using Etra.StarterAssets.Interactables;

namespace Etra.StarterAssets.Source.Editor
{
    [CustomEditor(typeof(Target))]
    public class UnityEditorForInteractableTarget : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //Create Apply Gameplay Settings button under public enum variables
            Target tgt = target as Target;
            tgt.updateRopes();
        }
    }
}
