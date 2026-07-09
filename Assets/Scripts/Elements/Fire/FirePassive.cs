using UnityEngine;

public class FirePassive : PassiveAbility
{
    HealthComponent healthComponent;
    public sealed override void Init(GameObject owner, ElementData elementData)
    {
        base.Init(owner, elementData);
        
        owner.TryGetComponent(out healthComponent);
        owner.TryGetComponent(out Player player);
        FireElement element = player.GetSpecificElement<FireElement>();

        foreach (ActiveAbility ability in element.Actives)
        {
            ability.OnAfterLaunch += HealOnAbilityCast;
        }

    }

    public sealed override void Uninit(GameObject owner)
    {
        if (owner.TryGetComponent(out Player player))
        {
            FireElement element = player.GetSpecificElement<FireElement>();
            foreach (ActiveAbility ability in element.Actives)
            {
                ability.OnAfterLaunch -= HealOnAbilityCast;
            }
        }
        base.Uninit(owner);
    }

    public void HealOnAbilityCast()
    {
        healthComponent.Heal(healthComponent.MaxHealth * data.power);
    }
}
