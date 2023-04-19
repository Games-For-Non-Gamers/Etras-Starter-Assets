using UnityEngine;

namespace EtrasStarterAssets
{
    public class FPSTurretFist : MonoBehaviour
    {
        public FPSTurret mainScript;

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.tag == "Player")
            {
                mainScript.launchPlayer(other.gameObject);
            }
        }
    }
}
