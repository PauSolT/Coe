using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : BaseDamage
{
    List<HealthComponent> targetsToDamage = new List<HealthComponent>();

    public void Init(GameObject owner, ElementData element, float damage, float damageFrequency, float duration = 0.1f, bool onTarget = false)
    {
        BaseInit(owner, damage, element, duration);

        if (onTarget)
        {
            HealthComponent health = GetComponentInParent<HealthComponent>();
            targetsToDamage.Add(health);
        }

        TimerManager.Instance.StartTickTimer(duration,
            damageFrequency,
            onTick: () =>
            {
                foreach (HealthComponent health in targetsToDamage)
                {
                    health.TakeDamage(damageComponent);
                }
            },
            onEnd: SelfDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner(collision))
        {
            return;
        }

        if (collision.TryGetComponent(out HealthComponent health) && !targetsToDamage.Contains(health))
        {
            targetsToDamage.Add(health);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out HealthComponent health))
        {
            targetsToDamage.Remove(health);
        }
    }

}
