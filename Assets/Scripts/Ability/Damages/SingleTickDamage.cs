using System.Collections.Generic;
using UnityEngine;

public class SingleTickDamage : BaseDamage
{
    List<Collider2D> alreadyDamaged = new List<Collider2D>();

    public void Init(GameObject owner, ElementData element, float damage, float duration = 0.1f, bool onTarget = false)
    {
        BaseInit(owner, damage, element, duration);

        if (onTarget)
        {
            HealthComponent health = GetComponentInParent<HealthComponent>();
            health.TakeDamage(damageComponent);
        }

        TimerManager.Instance.StartTimer(duration, SelfDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner(collision) || alreadyDamaged.Contains(collision))
        {
            return;
        }

        alreadyDamaged.Add(collision);
        if (collision.TryGetComponent(out HealthComponent health))
        {
            CommonTriggerBehaviour(collision);
            health.TakeDamage(damageComponent);
        }
    }

}
