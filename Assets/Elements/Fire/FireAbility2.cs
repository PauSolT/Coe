using UnityEngine;

public class FireAbility2 : ActiveAbility
{
    protected sealed override void Execute()
    {
        //TODO
        //targeting system

        GameObject target = GameObject.Find("Enemy");

        Log.Info($"{owner.name} has executed {data.name}", LogCategory.Elements);
        GameObject instance = Object.Instantiate(
            data.prefab,
            target.transform.position,
            target.transform.localRotation);
        DamageOverTime damage = instance.GetComponentInChildren<DamageOverTime>();
        damage.Init(owner,
            data.power,
            data.tickFrequency,
            duration: data.duration);
    }

}
