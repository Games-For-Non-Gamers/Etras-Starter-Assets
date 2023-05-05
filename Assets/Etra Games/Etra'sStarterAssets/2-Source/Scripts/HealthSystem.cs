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

    public event Action<float> OnChange;
    public event Action<float> OnDamage;
    public event Action<float> OnHeal;
    public event Action OnDeath;

    #endregion

    void Start()
    {
      health = Mathf.Max(startingHealth, maxHealth);
    }

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