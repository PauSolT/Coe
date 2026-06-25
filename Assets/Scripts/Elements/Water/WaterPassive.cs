using System.Linq;
using UnityEngine;

public class WaterPassive : PassiveAbility
{

    public bool IsEmpowered { get; private set; } = true;
    WaterElement element;
    public sealed override void Init(GameObject owner)
    {
        base.Init(owner);

        if (owner.TryGetComponent(out Player player))
        {
            WaterElement element = player.GetSpecificElement<WaterElement>();
            foreach (WaterAbility ability in element.Actives.OfType<WaterAbility>())
            {
                ability.IsEmpowered = IsEmpowered;
                ability.OnAfterLaunch -= HasEmpowered;
            }
        }

    }

    public sealed override void Uninit(GameObject owner)
    {
        if (owner.TryGetComponent(out Player player))
        {
            WaterElement element = player.GetSpecificElement<WaterElement>();
            foreach (WaterAbility ability in element.Actives.OfType<WaterAbility>())
            {
                ability.OnAfterLaunch -= HasEmpowered;
            }
        }
        base.Uninit(owner);
    }

    private void HasEmpowered()
    {
        if (IsEmpowered)
        {
            Log.Info($"Just used an empowered water ability! {data.name}", LogCategory.Elements);
            IsEmpowered = false;
            TimerManager.Instance.StartTimer(data.cooldown, 
                () => { 
                    IsEmpowered = true;
                    SetAbilitiesEmpowered();
                });
        }
    }

    private void SetAbilitiesEmpowered()
    {
        Log.Info($"Next water ability is empowered!", LogCategory.Elements);
        foreach (WaterAbility ability in element.Actives)
        {
            ability.IsEmpowered = IsEmpowered;
        }
    }
   
}
