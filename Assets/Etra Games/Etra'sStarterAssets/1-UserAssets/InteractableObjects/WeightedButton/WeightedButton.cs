using Etra.StarterAssets.Source.Combat;
using EtrasStarterAssets;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class WeightedButton : MonoBehaviour
    {
        // References
        public SciFiDoor door;
        private Transform buttonMain;

        // Variables
        Vector3 buttonStartPos = new Vector3(0, -0.94f, 0);
        Vector3 buttonEndPos = new Vector3(0, -1.32f, 0);
        int numObjects = 0; // Track the number of objects on the button

        EtrasStarterAssets.AudioManager audioManager;

        private void Start()
        {
            audioManager = GetComponent<EtrasStarterAssets.AudioManager>();
            buttonMain = transform.GetChild(0);
            var buttonStartPos = buttonMain.transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" || (other.GetComponent<Rigidbody>() != null && other.GetComponent<BulletProjectile>() == null))
            {
                numObjects++; // New object on button
                buttonPressed();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player" || (other.GetComponent<Rigidbody>() != null && other.GetComponent<BulletProjectile>() == null))
            {
                numObjects--; // Object removed from button
                if (numObjects <= 0)
                {
                    numObjects = 0;
                    doorClose();
                }
            }
        }

        public void doorClose()
        {
            buttonReleased();
            door.SetOpened(false);
        }

        public void buttonPressed()
        {
            if (numObjects == 1)
            {
                audioManager.Play("ButtonPress");
                audioManager.Play("ButtonClick");
                audioManager.Stop("ButtonRelease");
                door.SetOpened(true);
                LeanTween.moveLocal(buttonMain.gameObject, buttonEndPos, 0.15f);
            }
        }

        public void buttonReleased()
        {
            audioManager.Play("ButtonRelease");
            audioManager.Stop("ButtonPress");
            LeanTween.moveLocal(buttonMain.gameObject, buttonStartPos, 0.15f);
        }
    }
}