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
        AnimationCurve damageCurve;
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

            currentStep = currentStep % damageCurve.keys.Length;
            float damage = damageCurve.keys[currentStep].value;
            currentStep++;
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
            checkpointRespawn.teleportToCheckpoint();
            healthSystem.Heal(healthSystem.maxHealth, true);
            currentStep = 0;
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
