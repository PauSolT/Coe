using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelayedSingleDamage : MonoBehaviour
{
    List<HealthComponent> targetsToDamage = new List<HealthComponent>();
    GameObject owner;

    public void Init(GameObject owner, float damage, float duration = 0.1f, bool onTarget = false)
    {
        this.owner = owner;
        AbilityPrefabRoot root = GetComponentInParent<AbilityPrefabRoot>();

        if (onTarget)
        {
            HealthComponent health = GetComponentInParent<HealthComponent>();
            health.TakeDamage(damage);
        }

        TimerManager.Instance.StartTimer(duration, () =>
        {
            foreach (HealthComponent health in targetsToDamage)
            {
                health.TakeDamage(damage);
            }
            root.Destroy();
        });
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
