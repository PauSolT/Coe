using UnityEngine;

public class FirePassive : PassiveAbility
{
    HealthComponent healthComponent;
    public sealed override void Init(GameObject owner)
    {
        base.Init(owner);
        
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
