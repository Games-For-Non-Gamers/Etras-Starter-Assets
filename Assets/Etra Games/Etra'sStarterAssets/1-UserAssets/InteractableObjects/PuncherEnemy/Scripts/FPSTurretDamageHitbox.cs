using UnityEngine;

namespace EtrasStarterAssets
{
    public class FPSTurretDamageHitbox : MonoBehaviour, IDamageable<int>
    {
        public FPSTurret mainScript;
        public void TakeDamage(int damage)
        {
            mainScript.takeDamage(damage);
        }
    }
}
