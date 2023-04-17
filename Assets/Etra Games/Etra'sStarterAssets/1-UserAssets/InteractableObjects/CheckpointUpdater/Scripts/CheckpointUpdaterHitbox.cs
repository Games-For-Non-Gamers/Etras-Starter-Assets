using UnityEngine;

namespace EtrasStarterAssets
{
    public class CheckpointUpdaterHitbox : MonoBehaviour
    {
        public CheckpointUpdater parent;
        [HideInInspector]public MeshRenderer renderer;

        private void Reset()
        {
            renderer = GetComponent<MeshRenderer>();
        }
        private void OnValidate()
        {
            renderer = GetComponent<MeshRenderer>();
        }
        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
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

