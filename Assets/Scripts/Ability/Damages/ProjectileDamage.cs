using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private GameObject owner;
    private float damage;

    Timer timer;  

    public void Init(GameObject owner, GameObject target, float damage, float maxDuration)
    {
        this.owner = owner;
        this.damage = damage;

        AbilityPrefabRoot root = GetComponentInParent<AbilityPrefabRoot>();

        timer = TimerManager.Instance.StartTimer(maxDuration,
            onEnd: () =>
            {
                root.SelfDestroy();
            },
            update: (deltaTime) =>
            {
                if (target == null)
                    return;

                Vector3 direction = (target.transform.position - transform.position).normalized;
                transform.position += deltaTime * speed * direction;
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner) return;

        if (collision.TryGetComponent(out HealthComponent health))
        {
            health.TakeDamage(damage);
            GetComponentInParent<AbilityPrefabRoot>().SelfDestroy();
            timer?.Cancel();
        }
    }
}
