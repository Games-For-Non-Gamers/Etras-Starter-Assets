using UnityEngine;

namespace EtrasStarterAssets{
    public class FPSTurretPlayerDetection : MonoBehaviour
    {
        public FPSTurret parentScript;
        public bool playerVisible = false;
        public bool playerInRange = false;

        //Time to commit a sin
        private void Update()
        {


        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                parentScript.playerDetected();
                playerInRange = true;
            }
        }




        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                parentScript.backToIdle();
                playerInRange = false;
            }
        }
    }
}
