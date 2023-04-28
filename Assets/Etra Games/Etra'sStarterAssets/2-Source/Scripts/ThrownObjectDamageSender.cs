using Etra.StarterAssets.Interactables.Enemies;
using UnityEngine;

namespace Etra.StarterAssets.Source
{
    //This script will be attached to moving objects that have not yet touched the ground and will deal damagte to hit objects
    public class ThrownObjectDamageSender : MonoBehaviour
    {
        public int objectDamage = 1; // this is editable by external scripts

        private void OnCollisionEnter(Collision collision)
        {
            //If IDamageable is noticed, add damage
            var isDamageableCheck = collision.gameObject.GetComponent<IDamageable<int>>();
            if (isDamageableCheck != null)
            {
                isDamageableCheck.TakeDamage(objectDamage);
            }

            //Reagardless of IDamageable, destroy this script
            Destroy(this);
        }
    }
}

