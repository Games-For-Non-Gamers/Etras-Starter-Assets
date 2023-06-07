using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;

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
        if (showInEditor)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (showInGame)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_ContinuousHealth>()?.Damage();
        }
    }
}
