using UnityEngine;

namespace EtrasStarterAssets
{
    public class CheckpointUpdaterTeleportLocation : MonoBehaviour
    {
        public MeshRenderer meshRenderer;

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
    }
}
