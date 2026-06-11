using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    List<HealthComponent> targetsToDamage = new List<HealthComponent>();
    GameObject owner;
    float damage;
    float damageDuration;
    float damageFrequency;

    public void Init(GameObject owner, float damage, float damageDuration, float damageFrequency, float duration = 0.1f)
    {
        this.owner = owner;
        this.damage = damage;
        this.damageDuration = damageDuration;
        this.damageFrequency = damageFrequency;

        TimerManager.Instance.StartTickTimer(duration,
            damageFrequency,
            onTick: () =>
            {
                foreach (HealthComponent health in targetsToDamage)
                {
                    health.TakeDamage(damage);
                }
            },
            onEnd: () => Destroy(transform.root.gameObject));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != owner)
        {
            collision.TryGetComponent(out HealthComponent health);
            if (health != null && !targetsToDamage.Contains(health))
            {
                targetsToDamage.Add(health);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != owner)
        {
            collision.TryGetComponent(out HealthComponent health);
            if (health != null && targetsToDamage.Contains(health))
            {
                targetsToDamage.Remove(health);
            }
        }
    }

}
