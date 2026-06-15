using System;
using UnityEngine;

public class ActiveAbility : Ability
{
    public Action OnBeforeLaunch;
    public Action OnAfterLaunch;

    protected GameObject owner;

    protected bool onCooldown = false;
    Timer cooldownTimer = null;


    public override void Init(GameObject owner)
    {
        base.Init(owner);
        this.owner = owner;
        OnAfterLaunch += StartCooldown;
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
        if (onCooldown)
        {
            Log.Info($"{data.name} is on cooldown! Needs {cooldownTimer.CurrentTime:0.##} more seconds", LogCategory.Elements);
            return;
        }

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

    /// <summary>
    /// Puts the ability on cooldown and starts the cooldown timer
    /// </summary>
    protected virtual void StartCooldown()
    {
        onCooldown = true;
        cooldownTimer = TimerManager.Instance.StartTimer(data.cooldown, () => onCooldown = false);
    }

}
