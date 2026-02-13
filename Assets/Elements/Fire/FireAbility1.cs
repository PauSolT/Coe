using UnityEngine;

public class FireAbility1 : ActiveAbility
{

    protected sealed override void Execute()
    {
        Log.Info(owner.name);
        GameObject instance = Object.Instantiate(
            data.prefab, 
            owner.transform.position, 
            owner.transform.localRotation);
        SingleTickDamage damage = instance.GetComponentInChildren<SingleTickDamage>();
        damage.Init(owner,
            data.power,
            data.duration);
    }

}
