using UnityEngine;

public class FirePassive : PassiveAbility
{
    HealthComponent healthComponent;
    public sealed override void Init(GameObject owner)
    {
        base.Init(owner);
        
        Log.Info($"Initializing fire passive with this data: {data}");
        Log.Info($"Owner {owner.name}");
        owner.TryGetComponent(out healthComponent);
        owner.TryGetComponent(out Player player);
        FireElement element = player.GetSpecificElement<FireElement>();

        foreach (ActiveAbility ability in element.Actives)
        {
            ability.OnBeforeLaunch += HealOnAbilityCast;
        }

    }

    public void HealOnAbilityCast()
    {
        healthComponent.Heal(healthComponent.MaxHealth * data.power);
    }
}
