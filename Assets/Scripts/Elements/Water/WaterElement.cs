using UnityEngine;

public class WaterElement : Element
{
    public override void Init()
    {
        //Adding passive and actives before base init 
        //to avoid null passive
        AddPassive(new WaterPassive());
        AddActive(new WaterAbility1());
        AddActive(new WaterAbility2());
        AddActive(new WaterAbility3());
        base.Init();

    }
}