using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class DoorAutoOpenHitbox : MonoBehaviour
    {
        Door parentDoor;

        private void Start()
        {
            parentDoor = GetComponentInParent<Door>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                parentDoor.doorInteract();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                parentDoor.doorInteract();
            }
        }
    }
}
