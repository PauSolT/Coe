public class FireAbility3 : ActiveAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();
        HealthComponent health = owner.GetComponent<HealthComponent>();
        health.Invulnerable = true;
        
        TimerManager.Instance.StartTimer(data.duration, () => { health.Invulnerable = false; });
        return true;
    }

}
