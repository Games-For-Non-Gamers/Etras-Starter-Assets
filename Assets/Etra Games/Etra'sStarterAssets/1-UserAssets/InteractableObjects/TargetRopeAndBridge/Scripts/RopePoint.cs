using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class RopePoint : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Target target;

        //This script exists so moving the rope point updates the overall rope line renderer in the editor
        private void Reset()
        {
            target = GetComponentInParent<Target>();
            OnBeforeSerialize();
        }
        public void OnBeforeSerialize()
        {
            if (target != null)
            {
                target.OnValidate();
            }
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
