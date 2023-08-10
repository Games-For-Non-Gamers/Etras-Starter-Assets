using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Etra.StarterAssets.Combat;
using Etra.StarterAssets.Source;
using UnityEngine.SceneManagement;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive, typeof(HealthSystem))]
    class ABILITY_Health : EtraAbilityBaseClass
    {
        [SerializeField, Header("Main variables")]
        float damageCooldownWaitTime = 0.5f;
        [SerializeField]
        float healCooldownWaitTime = 0f;
        bool damageCooldown = false, healCooldown = false;
        HealthSystem healthSystem;
        ABILITY_CheckpointRespawn checkpointRespawn;
        [HideInInspector]
        public GameObject healthFilter;
        Image image;
        float animationTime;


        public void Reset()
        {
            if (gameObject.name == "Tempcube") { return; }
            transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects(); //string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale
            if (GameObject.Find("DamageFilter"))
            {
                DestroyImmediate(GameObject.Find("DamageFilter"));
            }
            healthFilter = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("DamageFilter", gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
            //Set above the cursor, but behind everything else
            if (healthFilter.transform.parent.childCount > 1)
            {
                healthFilter.transform.SetSiblingIndex(1);
            }
        }

        public override void abilityStart()
        {

            image = healthFilter.GetComponent<Image>();

            healthSystem = GetComponentInChildren<HealthSystem>();
            healthSystem.OnDamage.AddListener(OnDamage);
            healthSystem.OnHeal.AddListener(OnHeal);
            healthSystem.OnDeath.AddListener(OnDeath);
            healthSystem.OnChange.AddListener(OnChange);

            if (GetComponent<ABILITY_CheckpointRespawn>())
            {
                checkpointRespawn = GetComponent<ABILITY_CheckpointRespawn>();
                animationTime = checkpointRespawn.animationTime * .5f;
            }

        }

        /// <summary>
        /// Damages the player.
        /// </summary>
        /// <param name="damage">The amount of damage the player will take.</param>
        public void Damage(float damage)
        {
            if (damageCooldown) return;

            healthSystem.Damage(damage);
        }

        /// <summary>
        /// Heals the player.
        /// </summary>
        /// <param name="health">How much health the player will gain.</param>
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
            StartCoroutine(HealCooldown());
        }

        void OnDeath()
        {
            if (checkpointRespawn)
            {
                healthSystem.manualDeath = true;
                checkpointRespawn.teleportToCheckpoint();
                StartCoroutine(Respawn());
            }
            else
            {
                //Reload current scene 
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        void OnChange(float change)
        {
            float healthLost = healthSystem.maxHealth - healthSystem.health;
            image.color = new Color(image.color.r, image.color.g, image.color.b, healthLost / healthSystem.maxHealth);
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

        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(animationTime);
            healthSystem.Heal(healthSystem.maxHealth, true);
            healthSystem.manualDeath = false;
        }
    }
}
