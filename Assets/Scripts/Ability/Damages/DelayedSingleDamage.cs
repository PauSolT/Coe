using System.Collections.Generic;
using UnityEngine;

public class DelayedSingleDamage : BaseDamage
{
    List<HealthComponent> targetsToDamage = new List<HealthComponent>();

    public void Init(GameObject owner, ElementData element, float damage, float duration = 0.1f, bool onTarget = false)
    {
        BaseInit(owner, damage, element, duration);

        if (onTarget && GetComponentInParent<HealthComponent>() is HealthComponent health)
        {
            health.TakeDamage(damageComponent);
        }

        TimerManager.Instance.StartTimer(duration, () =>
        {
            foreach (HealthComponent health in targetsToDamage)
            {
                health.TakeDamage(damageComponent);
            }
            SelfDestroy();
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner(collision))
        {
            return;
        }

        if (collision.TryGetComponent(out HealthComponent health) && !targetsToDamage.Contains(health))
        {
            damageComponent.SetHitFromRight(IsDamageFromRight(collision));
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
