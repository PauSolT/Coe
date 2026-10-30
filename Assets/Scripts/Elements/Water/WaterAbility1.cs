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

        GameObject instance = Object.Instantiate(
            data.prefab,
            owner.transform.position,
            owner.transform.localRotation);
        ProjectileDamage damage = instance.GetComponentInChildren<ProjectileDamage>();
        damage.Init(owner,
            elementData,
            GetCurrentTarget(),
            finalPower,
            finalDuration);

        return true;
    }

}
