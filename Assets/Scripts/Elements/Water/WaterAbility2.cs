using UnityEngine;

public class WaterAbility2 : WaterAbility
{
    float finalSecondaryPower;
    GameObject instance;
    protected sealed override bool Execute()
    {
        base.Execute();

        float finalPower = data.power;
        finalSecondaryPower = data.secondaryPower;
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
            health.AddShield("WaterAbility2", finalPower, finalDuration, Damage);
        }


        return true;
    }

    private void Damage()
    {
        DelayedSingleDamage damage = instance.GetComponentInChildren<DelayedSingleDamage>();
        damage.Init(owner, finalSecondaryPower, 0.05f);
    }

}
