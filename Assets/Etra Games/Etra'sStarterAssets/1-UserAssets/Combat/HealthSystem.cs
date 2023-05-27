using System;
using UnityEngine;
using UnityEngine.Events;

namespace Etra.StarterAssets.Combat
{

  public sealed class HealthSystem : MonoBehaviour
  {
    #region Variables
    [Header("Variables")]
    [SerializeField, Tooltip("The maximum health of the object")] float maxHealth;
    [SerializeField, Tooltip("The starting health of the object")] float startingHealth;
    /**
      <summary>
        If true, <see cref="OnDeath"/> won't be called.
      </summary>
    */
    [Tooltip("If true, On Death () won't be called")] public bool manualDeath;
    float _health;
    /**
      <summary>
        The health of the GameObject.
      </summary>
      <remarks>
        <para>
          This variable's setter isn't public because it should be set by using the <see cref="Damage"/> and <see cref="Heal"/> methods.
        </para>
      </remarks>
    */
    public float health
    {
      get
      {
        return _health;
      }
      private set
      {
        float lastHealth = _health;
        _health = Mathf.Clamp(value, 0, maxHealth);
        OnChange?.Invoke(Mathf.Abs(_health - lastHealth));
        if (manualDeath)
          return;

        bool last = isAlive;
        isAlive = _health > 0;
        if (last && !isAlive) OnDeath?.Invoke();
      }
    }

    public bool isAlive { get; private set; }

    #endregion

    #region Events
    [Header("Events")]
    /**
      <summary>
        Called when the health changes.
      </summary>
    */
    [Tooltip("Called when the health changes")] public UnityEvent<float> OnChange;
    /**
      <summary>
        Called when the object is damaged.
      </summary>
    */
    [Tooltip("Called when the object is damaged")] public UnityEvent<float> OnDamage;
    /**
      <summary>
        Called when the object is healed.
      </summary>
    */
    [Tooltip("Called when the object is healed")] public UnityEvent<float> OnHeal;
    /**
      <summary>
        Called when the health becomes 0.
      </summary>
    */
    [Tooltip("Called when the health becomes 0")] public UnityEvent OnDeath;

    #endregion

    #region Methods

    /**
      <summary>
        Decreases the amount of health.
      </summary>
      <param name="hp">Amount of health to decrease. Negative values will be evaluated as positive.</param>
      <returns>The amount of health that was decreased.</returns>
    */
    public void Damage(float hp)
    {
      float lastHealth = health;
      health -= Mathf.Abs(hp);
      OnDamage?.Invoke(Mathf.Abs(_health - lastHealth));
    }

    /**
      <summary>
        Increases the amount of health.
      </summary>
      <param name="hp">Amount of health to increase. Negative values will be evaluated as positive.</param>
      <returns>The amount of health that was increased.</returns>
    */
    public void Heal(float hp)
    {
      float lastHealth = health;
      health += Mathf.Abs(hp);
      OnHeal?.Invoke(Mathf.Abs(_health - lastHealth));
    }

    #endregion

    #region Unity Methods

    void Start()
    {
      health = Mathf.Min(startingHealth, maxHealth);
    }

    #endregion

  }


}
