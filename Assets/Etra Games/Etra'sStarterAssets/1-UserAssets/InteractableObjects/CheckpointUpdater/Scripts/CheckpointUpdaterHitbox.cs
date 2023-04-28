using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class CheckpointUpdaterHitbox : MonoBehaviour
    {
        public CheckpointUpdater parent;
        [HideInInspector] public MeshRenderer meshRenderer;

        private void Reset()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        private void OnValidate()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                parent.hitboxHitByPlayer();
            }
        }
    }
}

