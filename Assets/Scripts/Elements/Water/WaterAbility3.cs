using UnityEngine;

public class WaterAbility3 : WaterAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();
        GameObject instance = Object.Instantiate(
            data.prefab, 
            owner.transform.position, 
            owner.transform.localRotation,
            owner.transform);
        DamageOverTime damage = instance.GetComponentInChildren<DamageOverTime>();
        damage.Init(owner,
            data.power,
            data.tickFrequency,
            duration: data.duration);

        return true;
    }

}
