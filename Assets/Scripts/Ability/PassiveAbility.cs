using UnityEngine;

public class PassiveAbility : Ability
{
    public override void Init(GameObject owner, ElementData elementData)
    {
        base.Init(owner, elementData);
    }

    public override void Uninit(GameObject owner)
    {
        base.Uninit(owner);
    }
}
