using UnityEngine;
using UnityEditor;

namespace EtrasStarterAssets{
    [CustomEditor(typeof(INTERACTABLE_Target))]
    public class UnityEditorForInteractableTarget : Editor
    {
       
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //Create Apply Gameplay Settings button under public enum variables
            INTERACTABLE_Target tgt = target as INTERACTABLE_Target;
            tgt.updateRopes();
            
            
        }
        
    }
}
