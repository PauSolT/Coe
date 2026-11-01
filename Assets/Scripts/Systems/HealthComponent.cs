using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ElementalReceiver))]
public class HealthComponent : MonoBehaviour, ITargetable
{
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    [field: SerializeField] public float CurrentHealth { get; private set; } = 10f;

    [field: SerializeField] public bool Invulnerable { get; set; } = false;

    [field: SerializeField] public bool IsTargetable { get; private set; } = true;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnHit;

    [SerializeField] private List<ShieldComponent> activeShields = new List<ShieldComponent>();

    public GameObject popupPrefab;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        CurrentHealth = MaxHealth;
        activeShields.Clear();
    }

    /// <summary>
    /// User takes damage.
    /// </summary>
    /// <param name="damage">Amount of damage the user takes.</param>
    /// <returns>Returns the damage dealt to health.</returns>
    public void TakeDamage(DamageComponent damage)
    {
        if (damage.ElementData != null)
        {
            ElementalReceiver receiver = GetComponent<ElementalReceiver>();
            float reactionBonus = receiver.ApplyElement(damage.ElementData);
            damage.SetMultiplier(reactionBonus);
        }

        Log.Info($"{gameObject.name} Taking {damage.GetTotalAmount()} damage ()", LogCategory.Health, this);
        DecreaseHealth(damage);
    }


    /// <summary>
    /// Adds a shield to the user
    /// </summary>
    /// <param name="shieldId">Id of the- shield (normally the name of the ability)</param>
    /// <param name="amount">Amount of shield (power of the ability)</param>
    /// <param name="duration">Duration of the shield. If infinite, -1</param>
    /// <param name="onExpired">What to do when the shield expires (either damage or duration)</param>
    public void AddShield(string shieldId, float amount, float duration = -1f, Action onExpired = null)
    {
        ShieldComponent existingShield = activeShields.Find(s => s.Id == shieldId);

        if (existingShield != null)
        {
            // Refresh the existing shield amount
            existingShield.Amount = amount;

            // If it had a timer running, cancel it so we can restart the duration
            if (existingShield.ShieldTimer != null)
            {
                existingShield.ShieldTimer.Cancel();
                existingShield.ShieldTimer = null;
            }

            if (duration > 0f)
            {
                existingShield.ShieldTimer = TimerManager.Instance.StartTimer(duration, () => ExpireShield(existingShield));
            }

            Log.Info($"Refreshed Shield {shieldId} to {amount}", LogCategory.Health, this);
        }
        else
        {
            ShieldComponent newShield = new ShieldComponent(shieldId, amount, onExpired);

            if (duration > 0f)
            {
                newShield.ShieldTimer = TimerManager.Instance.StartTimer(duration, () => ExpireShield(newShield));
            }

            activeShields.Add(newShield);
            Log.Info($"Added Shield {shieldId} for {amount}", LogCategory.Health, this);
        }
    }

    /// <summary>
    /// Called automatically by the TimerManager when a shield's time runs out.
    /// </summary>
    private void ExpireShield(ShieldComponent shield)
    {
        if (activeShields.Contains(shield))
        {
            activeShields.Remove(shield);
            shield.OnDestroyedOrExpired?.Invoke();
        }
    }

    /// <summary>
    /// Decreases the health of the user.
    /// </summary>
    /// <param name="damage">Damage to subtract to health.</param>
    /// <returns>Returns the effective damage dealt to health.</returns>
    public float DecreaseHealth(DamageComponent damage)
    {
        OnHit?.Invoke();
        if (Invulnerable) return -1f;

        float totalDamage = damage.GetTotalAmount();
        float remainingDamage = totalDamage;


        // Iterate backwards because we are removing items from the list
        for (int i = activeShields.Count - 1; i >= 0; i--)
        {
            ShieldComponent shield = activeShields[i];

            if (shield.Amount > remainingDamage)
            {
                // Damage fully absorbed
                shield.Amount -= remainingDamage;
                remainingDamage = 0;
                break; 
            }
            else
            {
                remainingDamage -= shield.Amount;

                shield.ShieldTimer?.Cancel();
                shield.OnDestroyedOrExpired?.Invoke();

                activeShields.RemoveAt(i);
            }
        }

        // Apply leftover damage to actual health
        if (remainingDamage > 0)
        {
            CurrentHealth -= remainingDamage;
            OnHealthChanged?.Invoke(CurrentHealth, remainingDamage);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        if (totalDamage != remainingDamage)
        {
            float shieldDamage = totalDamage - remainingDamage;
            GameObject popupShield = Instantiate(popupPrefab, transform.position, Quaternion.identity);
            popupShield.GetComponentInChildren<DamageText>().Init(shieldDamage, true, damage);
        }
        if (remainingDamage > 0)
        {
            GameObject popupDamage = Instantiate(popupPrefab, transform.position, Quaternion.identity);
            popupDamage.GetComponentInChildren<DamageText>().Init(remainingDamage, false, damage);
        }

        return remainingDamage;
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

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
