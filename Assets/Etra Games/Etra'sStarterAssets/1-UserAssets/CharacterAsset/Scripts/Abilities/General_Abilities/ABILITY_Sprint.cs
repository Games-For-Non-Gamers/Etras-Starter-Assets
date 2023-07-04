using UnityEngine;

namespace Etra.StarterAssets.Abilities
{
    [RequireComponent(typeof(ABILITY_CharacterMovement))]

    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Sprint : EtraAbilityBaseClass
    {
        //This script is just a contaiiner for the sprint speed variable.
        //On Validate it will set the public var sprintSpeed to Character_Movement's sprint speed.
        public float sprintSpeed = 6;
        ABILITY_CharacterMovement movementAbility;

        public override void abilityStart()
        {
            movementAbility = GetComponent<ABILITY_CharacterMovement>();
        }

        public override void abilityUpdate()
        {
            //Every frame is annoying, but currently only way to make dynamic until a switch to function
            if (abilityEnabled)
            {
                movementAbility.sprintSpeed= sprintSpeed;
            }
        }


        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            movementAbility = GetComponent<ABILITY_CharacterMovement>();
            if (abilityEnabled)
            {
                movementAbility.sprintSpeed = sprintSpeed;
            }
        }
    }
}
