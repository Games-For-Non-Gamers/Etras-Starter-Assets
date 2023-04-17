using UnityEngine;

namespace EtrasStarterAssets
{
    public class CheckpointUpdaterTeleportLocation : MonoBehaviour
    {
        public MeshRenderer renderer;

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
    }
}
