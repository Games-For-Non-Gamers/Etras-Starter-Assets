using Etra.StarterAssets.Abilities;
using UnityEngine;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive)]
    public class ABILITY_RigidbodyPush : EtraAbilityBaseClass
    {
        [Header("Basics")]
        public LayerMask pushLayers = new LayerMask();
        [Range(0.5f, 5f)] public float strength = 1.1f;

        public void Reset()
        {
            pushLayers = LayerMask.GetMask("Default");
        }

        public void PushRigidBodies(ControllerColliderHit hit)
        {

            if (!abilityEnabled)
            {
                return;
            }

            // https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

            // make sure we hit a non kinematic rigidbody
            var body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            // make sure we only push desired layer(s)
            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayers.value) == 0) return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            // Calculate push direction from move direction, horizontal motion only
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

            // Apply the push and take strength into account
            body.AddForce(pushDir * strength, ForceMode.Impulse);
        }
    }
}
