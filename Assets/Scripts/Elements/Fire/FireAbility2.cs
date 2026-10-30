using UnityEngine;

public class FireAbility2 : ActiveAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();
       
        if (!HasTarget())
        {
            return false;
        }

        GameObject target = GetCurrentTarget();

        GameObject instance = Object.Instantiate(
            data.prefab,
            target.transform.position,
            target.transform.localRotation,
            target.transform);
        DamageOverTime damage = instance.GetComponentInChildren<DamageOverTime>();
        damage.Init(owner,
            elementData,
            data.power,
            data.tickFrequency,
            duration: data.duration,
            onTarget: true);

        return true;

    }

}
