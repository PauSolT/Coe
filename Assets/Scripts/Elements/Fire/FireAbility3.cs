public class FireAbility3 : ActiveAbility
{
    protected sealed override void Execute()
    {
        Log.Info($"{owner.name} has executed {data.name}", LogCategory.Elements);
        HealthComponent health = owner.GetComponent<HealthComponent>();
        health.Invulnerable = true;
        
        TimerManager.Instance.StartTimer(data.duration, () => { health.Invulnerable = false; });
    }

}
