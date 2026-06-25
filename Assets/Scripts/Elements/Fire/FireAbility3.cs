using UnityEngine;

public class FireAbility3 : ActiveAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();
        if (!owner.TryGetComponent(out HealthComponent health))
        {
            return false;
        }

        health.Invulnerable = true;
        TimerManager.Instance.StartTimer(data.duration, () => { health.Invulnerable = false; });
        return true;
    }
}
