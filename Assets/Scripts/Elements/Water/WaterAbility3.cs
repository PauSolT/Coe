using UnityEngine;

public class WaterAbility3 : WaterAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();

        float finalPower = data.power;
        float finalSecondaryPower = data.secondaryPower;
        float finalDuration = data.duration;

        if (IsEmpowered)
        {
            finalDuration += data.empoweredDuration;
            finalPower += data.empoweredPower;
            finalSecondaryPower += data.empoweredSecondaryPower;
        }

        GameObject instance = Object.Instantiate(
            data.prefab, 
            owner.transform.position, 
            owner.transform.localRotation,
            owner.transform);
        DamageOverTime damage = instance.GetComponentInChildren<DamageOverTime>();
        damage.Init(owner,
            elementData,
            finalPower,
            data.tickFrequency,
            duration: finalDuration);

        return true;
    }

}
