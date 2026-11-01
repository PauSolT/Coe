using UnityEngine;

public class ProjectileDamage : BaseDamage
{
    [SerializeField] private float speed = 10f;
    Timer timer;

    public void Init(GameObject owner, ElementData element, GameObject target, float damage, float duration)
    {
        BaseInit(owner, damage, element, duration);

        timer = TimerManager.Instance.StartTimer(duration,
            onEnd: SelfDestroy,
            update: (deltaTime) =>
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                transform.position += deltaTime * speed * direction;
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner(collision))
        {
            return;
        }

        if (collision.TryGetComponent(out HealthComponent health))
        {
            CommonTriggerBehaviour(collision);
            health.TakeDamage(damageComponent);
            SelfDestroy();
            timer?.Cancel();
        }
    }
}
