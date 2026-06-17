using UnityEngine;

public class WaterAbility2 : WaterAbility
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
        SingleTickDamage damage = instance.GetComponentInChildren<SingleTickDamage>();
        TimerManager.Instance.StartTimer(finalDuration, () => damage.Init(owner, finalPower, 0.05f));

        return true;
    }

}
