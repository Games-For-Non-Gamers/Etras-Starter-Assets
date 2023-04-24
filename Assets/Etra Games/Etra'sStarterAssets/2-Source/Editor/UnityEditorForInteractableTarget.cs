using UnityEngine;
using UnityEditor;

namespace EtrasStarterAssets{
    [CustomEditor(typeof(Target))]
    public class UnityEditorForInteractableTarget : Editor
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
