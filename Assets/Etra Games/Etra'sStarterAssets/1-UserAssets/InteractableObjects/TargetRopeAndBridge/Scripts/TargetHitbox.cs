using Etra.StarterAssets.Interactables.Enemies;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class TargetHitbox : MonoBehaviour, IDamageable<int>
    {
        //Recieve damage through the IDamageable interface
        Target target;

        public void TakeDamage(int damage)
        {
            target = GetComponentInParent<Target>();
            target.targetHit();
        }

    }
}
