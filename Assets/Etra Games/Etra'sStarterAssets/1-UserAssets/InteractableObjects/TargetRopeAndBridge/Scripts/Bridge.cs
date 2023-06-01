using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class Bridge : MonoBehaviour
    {
        //Variables
        public Target[] targets;
        public float timeToLower = 1;
        private bool bridgeLowered = false;
        EtrasStarterAssets.AudioManager audioManager;

        private void Start()
        {
            // Check this at start so the bridge automatically lowers if no targets are connected
            checkActivate();
            audioManager = GetComponentInChildren<EtrasStarterAssets.AudioManager>();
        }

        //This is called each time as connected target is destoryed
        public void checkActivate()
        {
            if (bridgeLowered)
            {
                return;
            }

            bool allActivated = true;
            for (int i = 0; i < targets.Length; i++)
            {
                if (!targets[i].activated)
                {
                    allActivated = false;
                }
            }

            if (allActivated)
            {
                StartCoroutine(lowerBridge());
            }

        }

        //Animation for bridge lowering
        IEnumerator lowerBridge()
        {
            bridgeLowered = true;
            //Second delay before the bridge lowers
            yield return new WaitForSeconds(1);
            audioManager.Play("BridgeFall");
            //Rotate correct anchor point
            LeanTween.rotate(transform.GetChild(0).GetChild(0).gameObject, Vector3.zero, timeToLower).setEaseInOutSine();
            yield return new WaitForSeconds(1);
            audioManager.Play("MedSparkle");
        }

    }
}

