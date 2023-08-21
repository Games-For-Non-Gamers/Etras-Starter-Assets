using Etra.StarterAssets.Input;
using EtrasStarterAssets;
using UnityEngine;

namespace Etra.StarterAssets.Abilities.FirstPerson
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Dash : EtraAbilityBaseClass
    {
        [Header("Basic")]
        public float dashRange = 70.0f;
        public float dashCooldown = 1f;
        public int damageFromDash = 2;

        //private
        private float dashTimeoutDelta = 0;
        private bool cooling;

        // References
        private StarterAssetsInputs inputs;
        private AudioManager abilitySoundManager;

        public override void abilityStart()
        {
            inputs = GetComponentInParent<StarterAssetsInputs>();
            mainController = GetComponentInParent<EtraCharacterMainController>();

            // Get SFX manager
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
        }

        public override void abilityUpdate()
        {
            if (!abilityEnabled)
            {
                inputs.dash = false;
                return;
            }

            if (dashTimeoutDelta > 0.0f)
            {
                dashTimeoutDelta -= Time.deltaTime;
            }
            else if (dashTimeoutDelta < 0.0f && cooling)
            {
                cooling = false;
                inputs.dash = false;
            }

            if (cooling)
            {
                return;
            }

            if (inputs.dash)
            {
                // Perform the dash ability
                PerformDash();
            }
        }

        private void PerformDash()
        {
            // Apply the impulse force and damage
            EtraCharacterMainController.Instance.addImpulseForceWithDamageToEtraCharacter(transform.forward, dashRange, damageFromDash, dashCooldown / 2);

            // Play the dash sound effect
            abilitySoundManager.Play("Dash");

            // Set the dash cooldown and mark as cooling down
            dashTimeoutDelta = dashCooldown;
            cooling = true;
        }
    }
}
