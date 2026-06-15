using System.Collections.Generic;
using UnityEngine;

public class SingleTickDamage : MonoBehaviour
{
    List<Collider2D> alreadyDamaged = new List<Collider2D>();
    GameObject owner;
    float damage;

    public void Init(GameObject owner, float damage, float duration = 0.1f, bool onTarget = false)
    {
        this.owner = owner;
        this.damage = damage;
        AbilityPrefabRoot root = GetComponentInParent<AbilityPrefabRoot>();

        if (onTarget)
        {
            HealthComponent health = GetComponentInParent<HealthComponent>();
            health.TakeDamage(damage);
        }

        TimerManager.Instance.StartTimer(duration, () => root.Destroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!alreadyDamaged.Contains(collision) && collision.gameObject != owner)
        {
            alreadyDamaged.Add(collision);
            collision.TryGetComponent(out HealthComponent health);
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

}
