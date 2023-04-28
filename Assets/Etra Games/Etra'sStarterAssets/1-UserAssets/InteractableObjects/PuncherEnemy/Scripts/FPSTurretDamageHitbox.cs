using UnityEngine;

namespace Etra.StarterAssets.Interactables.Enemies
{
    public class FPSTurretDamageHitbox : MonoBehaviour, IDamageable<int>
    {
        //If this hitbox is hit, take damage through idamageable and send the info to the mainscript
        public FPSTurret mainScript;
        public void TakeDamage(int damage)
        {
            mainScript.takeDamage(damage);
        }
    }
}
