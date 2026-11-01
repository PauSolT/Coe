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

        if (!owner.GetComponent<PlayerController>().GetFacingRight())
        {
            Vector3 scale = instance.transform.localScale;
            scale.x *= -1f;
            instance.transform.localScale = scale;
        }

        SingleTickDamage damage = instance.GetComponentInChildren<SingleTickDamage>();
        damage.Init(owner,
            elementData,
            data.power,
            data.duration);

        return true;
    }

}
