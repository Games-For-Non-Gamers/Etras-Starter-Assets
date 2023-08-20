using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Combat;

namespace Etra.StarterAssets.Interactables
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(Collider))]
    public class DamageTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("Amount of damage to deal")]
        float damage = 25f;
        [Header("Rendering")]
        [SerializeField] bool showInEditor = true;
        [SerializeField] bool showInGame = false;

        MeshRenderer meshRenderer;
#if UNITY_EDITOR
        HealthSystem healthSystem;
#endif

        void Reset()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void OnValidate()
        {
            meshRenderer = GetComponent<MeshRenderer>();

            meshRenderer.enabled = showInEditor;
        }

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            meshRenderer.enabled = showInGame;
#if UNITY_EDITOR
            healthSystem = EtraCharacterMainController.Instance.etraAbilityManager.GetComponentInChildren<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.OnChange.AddListener(Log);
            }
#endif

        }

#if UNITY_EDITOR
        void OnDestroy()
        {
            if (healthSystem != null)
            {
                healthSystem.OnChange.RemoveListener(Log);
            }
        }

        void Log(float changed)
        {
            Debug.Log($"Current health: {healthSystem.health} Changed health: {changed}");
        }
#endif

        void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (!EtraCharacterMainController.Instance.etraAbilityManager.TryGetComponent<ABILITY_Health>(out ABILITY_Health ability))
                return;

            ability.Damage(damage);
        }
    }
}
