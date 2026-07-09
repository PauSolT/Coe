using UnityEngine;

public class FireAbility1 : ActiveAbility
{

    protected sealed override bool Execute()
    {
        base.Execute();
        GameObject instance = Object.Instantiate(
            data.prefab, 
            owner.transform.position, 
            owner.transform.localRotation);
        SingleTickDamage damage = instance.GetComponentInChildren<SingleTickDamage>();
        damage.Init(owner,
            element,
            data.power,
            data.duration);

        return true;
    }

}
