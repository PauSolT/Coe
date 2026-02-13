using System;
using UnityEngine;

public class ActiveAbility : Ability
{
    public Action OnBeforeLaunch;
    public Action OnAfterLaunch;

    protected GameObject owner;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        this.owner = owner;
    }

    public override void Uninit()
    {
        base.Uninit();
    }

    /// <summary>
    /// Fires all the ability logic.
    /// </summary>
    public void AbilityUse()
    {
        OnBeforeLaunch?.Invoke();
        Execute();
        OnAfterLaunch?.Invoke();
    }

    /// <summary>
    /// Contains the ability logic.
    /// </summary>
    protected virtual void Execute()
    {
        
    }

}
