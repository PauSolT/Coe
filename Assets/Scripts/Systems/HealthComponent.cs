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
    /// User takes damage.
    /// </summary>
    /// <param name="damage">Amount of damage the user takes.</param>
    /// <returns>Returns the damage dealt to health.</returns>
    public void TakeDamage(float damage)
    {
        Log.Info($"{gameObject.name} Taking {damage} damage", LogCategory.Health, this);
        DecreaseHealth(damage);
    }

    /// <summary>
    /// Decreases the health of the user.
    /// </summary>
    /// <param name="damage">Damage to subtract to health.</param>
    /// <returns>Returns the effective damage dealt to health.</returns>
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
    /// Incerases the health of the user.
    /// </summary>
    /// <param name="healing">Heal to add to health.</param>
    /// <returns>Returns the effective healing done.</returns>
    public float Heal(float healing)
    {
        Log.Info($"Healing {healing}", LogCategory.Health, this);
        float previousHealth = CurrentHealth;

        CurrentHealth += healing;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        return CurrentHealth - previousHealth;

    }


    /// <summary>
    /// When the user reaches 0 hp.
    /// </summary>
    void Die()
    {
        OnDeath?.Invoke();
    }



}
