using StarterAssets;
using UnityEngine;

public abstract class EtraAbilityBaseClass : MonoBehaviour
{
    public bool abilityEnabled = true;
    protected EtraCharacterMainController mainController;
    public virtual void abilityStart() { }
    public virtual void abilityUpdate() { }
    public virtual void abilityLateUpdate() { }

    public virtual EtraCharacterMainController.GameplayTypeFlags GameplayType { get; } = EtraCharacterMainController.GameplayTypeFlags.All;
}
