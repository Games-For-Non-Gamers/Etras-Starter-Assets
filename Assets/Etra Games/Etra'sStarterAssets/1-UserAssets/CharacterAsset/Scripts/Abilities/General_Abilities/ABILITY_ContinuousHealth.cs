using System;
using System.Collections;
using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Combat;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive, typeof(HealthSystem)), RequireComponent(typeof(ABILITY_CheckpointRespawn))]
    class ABILITY_ContinuousHealth : EtraAbilityBaseClass
    {
        [SerializeField]
        AnimationCurve curve;
        [SerializeField]
        float damageCooldownWaitTime, healCooldownWaitTime;
        bool damageCooldown = false, healCooldown = false;
        int currentStep = 0;
        HealthSystem healthSystem;
        ABILITY_CheckpointRespawn checkpointRespawn;
        public override void abilityStart()
        {
            healthSystem = GetComponentInChildren<HealthSystem>();
            checkpointRespawn = GetComponent<ABILITY_CheckpointRespawn>();
            healthSystem.OnDamage.AddListener(OnDamage);
            healthSystem.OnHeal.AddListener(OnHeal);
            healthSystem.OnDeath.AddListener(OnDeath);
        }

        public void Damage()
        {
            if (damageCooldown) return;

            float damage = curve.keys[currentStep].value <= curve.keys.Length ? curve.keys[currentStep].value : curve.keys[curve.keys.Length - 1].value;
            healthSystem.Damage(damage);
        }

        public void Heal(float health)
        {
            if (healCooldown) return;

            healthSystem.Heal(health);
        }

        void OnDamage(float damageDealt)
        {
            StartCoroutine(DamageCooldown());
        }

        void OnHeal(float healedHealth)
        {
            currentStep = 0;
            StartCoroutine(HealCooldown());
        }

        void OnDeath()
        {
            currentStep = 0;
            checkpointRespawn.teleportToCheckpoint();
            healthSystem.Heal(healthSystem.maxHealth);
        }

        IEnumerator DamageCooldown()
        {
            damageCooldown = true;
            yield return new WaitForSeconds(damageCooldownWaitTime);
            damageCooldown = false;
        }

        IEnumerator HealCooldown()
        {
            healCooldown = true;
            yield return new WaitForSeconds(healCooldownWaitTime);
            healCooldown = false;
        }
    }
}
