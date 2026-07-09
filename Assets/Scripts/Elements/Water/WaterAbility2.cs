using UnityEngine;

public class WaterAbility2 : WaterAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();

        GameObject instance;
        float finalPower = data.power;
        float finalSecondaryPower = data.secondaryPower;
        float finalDuration = data.duration;

        if (IsEmpowered)
        {
            finalDuration += data.empoweredDuration;
            finalPower += data.empoweredPower;
            finalSecondaryPower += data.empoweredSecondaryPower;
        }

        if (owner.transform.TryGetComponent(out HealthComponent health))
        {
            instance = Object.Instantiate(
            data.prefab,
            owner.transform.position,
            owner.transform.localRotation,
            owner.transform);
            health.AddShield("WaterAbility2", finalPower, finalDuration, () =>
            {
                DelayedSingleDamage damage = instance.GetComponentInChildren<DelayedSingleDamage>();
                damage.Init(owner, element, finalSecondaryPower, 0.05f);
            });
        }

        return true;
    }

}
