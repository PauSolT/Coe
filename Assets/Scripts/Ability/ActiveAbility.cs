using System;
using UnityEngine;

public class ActiveAbility : Ability
{
    public Action OnBeforeLaunch;
    public Action OnAfterLaunch;

    protected GameObject owner;
    protected ElementData element;

    protected bool onCooldown = false;
    Timer cooldownTimer = null;


    public override void Init(GameObject owner, ElementData element)
    {
        base.Init(owner, element);
        this.owner = owner;
        this.element = element;
        OnAfterLaunch += StartCooldown;
    }

    public override void Uninit(GameObject owner)
    {
        base.Uninit(owner);
        OnAfterLaunch -= StartCooldown;
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
        if (Execute())
        {
            OnAfterLaunch?.Invoke();
        }
    }

    /// <summary>
    /// Contains the ability logic.
    /// </summary>
    protected virtual bool Execute()
    {
        Log.Info($"{owner.name} has executed {data.name}", LogCategory.Elements);
        return true;
    }

    /// <summary>
    /// Puts the ability on cooldown and starts the cooldown timer
    /// </summary>
    protected virtual void StartCooldown()
    {
        onCooldown = true;
        cooldownTimer = TimerManager.Instance.StartTimer(data.cooldown, () => onCooldown = false);
    }

    /// <summary>
    /// Checks if the ability has a valid target
    /// </summary>
    /// <returns>Returns a bool</returns>
    protected bool HasTarget()
    {
        ITargetable targetable = TargetSystem.Instance.CurrentTarget;

        if (targetable == null)
        {
            Log.Info($"No target selected for ability {data.name}! {data.name} will not be used!", LogCategory.Elements);
        }
        return targetable != null;
    }

    /// <summary>
    /// Gets the current target
    /// </summary>
    /// <returns>Returns the gameObject of the target</returns>
    protected GameObject GetCurrentTarget()
    {
        return TargetSystem.Instance.CurrentTarget.GetGameObject();
    }

}
