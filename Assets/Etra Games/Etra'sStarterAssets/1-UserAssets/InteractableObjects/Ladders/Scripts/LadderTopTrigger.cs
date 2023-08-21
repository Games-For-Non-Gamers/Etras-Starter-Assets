using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{

    public class LadderTopTrigger : MonoBehaviour
    {
        public bool isOverlappingPlayer = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isOverlappingPlayer = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isOverlappingPlayer = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isOverlappingPlayer = false;
            }
        }
    }
}