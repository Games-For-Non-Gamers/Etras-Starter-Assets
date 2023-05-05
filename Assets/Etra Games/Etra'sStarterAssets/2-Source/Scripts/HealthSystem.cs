using System;
using UnityEngine;

namespace Etra.StarterAssets
{

  public sealed class HealthSystem : MonoBehaviour
  {
    #region Variables

    [SerializeField] float maxHealth;
    [SerializeField] float startingHealth;
    public bool manualIsAlive;
    float _health;
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
        if (!manualIsAlive)
        {
          bool last = isAlive;
          isAlive = _health > 0;
          if (last != isAlive && !isAlive) OnDeath?.Invoke();
        }
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


    #region Unity Methods

    void Start()
    {
      health = Mathf.Min(startingHealth, maxHealth);
    }

    #endregion


    /**
      <summary>
        Decreases the amount of health.
      </summary>
      <param name="hp">Amount of health to decrease.</param>
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
      <param name="hp">Amount of health to increase.</param>
      <returns>The amount of health that was increased.</returns>
    */
    public void Heal(float hp)
    {
      float lastHealth = health;
      health += Mathf.Abs(hp);
      OnHeal?.Invoke(Mathf.Abs(_health - lastHealth));
    }

  }


}
