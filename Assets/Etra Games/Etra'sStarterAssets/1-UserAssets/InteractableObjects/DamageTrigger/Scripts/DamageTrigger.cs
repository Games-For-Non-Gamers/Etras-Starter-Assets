using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(Collider))]
public class DamageTrigger : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] bool showInEditor = true;
    [SerializeField] bool showInGame = false;

    MeshRenderer meshRenderer;

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
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_ContinuousHealth>()?.Damage();
        }
    }
}
