using System.Collections.Generic;
using UnityEngine;

public class SingleTickDamage : MonoBehaviour
{
    List<Collider2D> alreadyDamaged = new List<Collider2D>();
    GameObject owner;
    float damage;

    public void Init(GameObject owner, float damage, float duration = 0.1f)
    {
        this.owner = owner;
        this.damage = damage;

        TimerManager.Instance.StartTimer(duration, () => Destroy(transform.root.gameObject));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!alreadyDamaged.Contains(collision) && collision.gameObject != owner)
        {
            alreadyDamaged.Add(collision);
            collision.TryGetComponent(out HealthComponent health);
            health.TakeDamage(damage);
        }
    }

}
