using UnityEngine;

public class WaterAbility1 : WaterAbility
{

    protected sealed override bool Execute()
    {
        base.Execute();
        if (!HasTarget())
        {
            return false;
        }
        float finalPower =  data.power;
        float finalSecondaryPower =  data.secondaryPower;
        float finalDuration = data.duration;

        if (IsEmpowered)
        {
            finalDuration += data.empoweredDuration;
            finalPower += data.empoweredPower;
            finalSecondaryPower += data.empoweredSecondaryPower;
        }

        GameObject target = GetCurrentTarget();
        GameObject instance = Object.Instantiate(
            data.prefab,
            target.transform.position,
            target.transform.localRotation);
        SingleTickDamage damage = instance.GetComponentInChildren<SingleTickDamage>();
        damage.Init(owner,
            finalPower,
            finalDuration);

        return true;
    }

}
