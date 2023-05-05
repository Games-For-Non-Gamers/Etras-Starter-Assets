using System;
using UnityEngine;

namespace Etra.StarterAssets
{

  public sealed class HealthSystem : MonoBehaviour
  {
    #region Variables

    [SerializeField] float maxHealth;
    [SerializeField] float startingHealth;
    /**
      <summary>
        If true, <see cref="OnDeath"/> won't be called.
      </summary>
    */
    public bool manualDeath;
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

    /**
      <summary>
        Called when the health changes.
      </summary>
    */
    public event Action<float> OnChange;
    /**
      <summary>
        Called when the health is damaged.
      </summary>
    */
    public event Action<float> OnDamage;
    /**
      <summary>
        Called when the health is healed.
      </summary>
    */
    public event Action<float> OnHeal;
    /**
      <summary>
        Called when the health becomes 0.
      </summary>
    */
    public event Action OnDeath;

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
