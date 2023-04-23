using UnityEngine;

namespace EtrasStarterAssets
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

