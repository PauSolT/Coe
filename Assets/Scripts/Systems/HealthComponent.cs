using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    [field: SerializeField] public float CurrentHealth { get; private set; } = 10f;


    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Decreases the health of the user.
    /// </summary>
    /// <param name="damage">Damage to subtract to health. It's in negative.</param>
    /// <returns>Returns the damage dealt to health.</returns>
    public float DecreaseHealth(float damage)
    {
        CurrentHealth -= damage;

        OnHealthChanged?.Invoke(CurrentHealth, damage);

        if (CurrentHealth < 0)
        {
            Die();
        }

        return CurrentHealth - damage;
    }


    /// <summary>
    /// When the user reaches 0 hp.
    /// </summary>
    void Die()
    {
        OnDeath?.Invoke();
    }

}
