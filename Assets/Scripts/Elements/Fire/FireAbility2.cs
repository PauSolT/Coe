using UnityEngine;

public class FireAbility2 : ActiveAbility
{
    protected sealed override void Execute()
    {
        ITargetable targetable = TargetSystem.Instance.CurrentTarget;

        if (targetable == null)
        {
            Log.Info($"No target selected for ability {data.name}! {data.name} will not be used!", LogCategory.Elements);
            return;
        }

        GameObject target = targetable.GetGameObject();

        Log.Info($"{owner.name} has executed {data.name}", LogCategory.Elements);
        GameObject instance = Object.Instantiate(
            data.prefab,
            target.transform.position,
            target.transform.localRotation,
            target.transform);
        DamageOverTime damage = instance.GetComponentInChildren<DamageOverTime>();
        damage.Init(owner,
            data.power,
            data.tickFrequency,
            duration: data.duration,
            onTarget: true);
    }

}
