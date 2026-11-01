using System.Collections.Generic;
using UnityEngine;

public class BaseDamage : MonoBehaviour
{
    protected GameObject owner;
    protected AbilityPrefabRoot root;
    protected float duration;

    protected DamageComponent damageComponent;

    protected void BaseInit(GameObject owner, float damage, ElementData element, float duration, float multiplier = 1)
    {
        this.owner = owner;
        this.duration = duration;
        root = GetComponentInParent<AbilityPrefabRoot>();

       damageComponent = new DamageComponent(damage, element, multiplier);

    }

    protected bool IsOwner(Collider2D collision)
    {
        return collision.gameObject == owner;
    }


    protected void SelfDestroy()
    {
        if (root != null)
        {
            root.SelfDestroy();
        }
    }

    protected bool IsDamageFromRight(Collider2D collision)
    {
        return owner.transform.position.x > collision.transform.position.x;
    }

    protected void CommonTriggerBehaviour(Collider2D collision)
    {
        damageComponent.TargetCollider = collision;
        damageComponent.SetHitFromRight(IsDamageFromRight(collision));
    }

}
