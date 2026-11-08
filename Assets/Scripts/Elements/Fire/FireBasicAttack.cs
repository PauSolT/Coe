using UnityEngine;

public class FireBasicAttack : ActiveAbility
{
    protected sealed override bool Execute()
    {
        base.Execute();
        if (!HasTarget())
        {
            return false;
        }

        GameObject instance = Object.Instantiate(
            data.prefab,
            owner.transform.position,
            owner.transform.localRotation);
        ProjectileDamage damage = instance.GetComponentInChildren<ProjectileDamage>();
        damage.Init(owner,
            elementData,
            GetCurrentTarget(),
            data.power,
            data.duration);

        return true;
    }
}