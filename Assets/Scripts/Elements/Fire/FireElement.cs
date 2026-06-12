using UnityEngine;

public class FireElement : Element
{
    public override void Init()
    {
        //Adding passive and actives before base init 
        //to avoid null passive
        AddPassive(new FirePassive());
        AddActive(new FireAbility1());
        AddActive(new FireAbility2());
        AddActive(new FireAbility3());
        base.Init();

    }
}
