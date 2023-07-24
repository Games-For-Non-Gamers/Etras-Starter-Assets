using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Combat;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(Collider))]
public class DamageTrigger : MonoBehaviour
{
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
        if (other.CompareTag("Player"))
        {
            EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_ContinuousHealth>()?.Damage();
            //TODO: add support for hp based health when it's implemented
        }
    }
}
