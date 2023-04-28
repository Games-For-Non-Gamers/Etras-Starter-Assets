using UnityEngine;

namespace Etra.StarterAssets.Interactables.Enemies
{
    public class FPSTurretPlayerDetection : MonoBehaviour
    {
        //Simply call a function and flip variables if player is visible
        public FPSTurret parentScript;
        public bool playerVisible = false;
        public bool playerInRange = false;

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
