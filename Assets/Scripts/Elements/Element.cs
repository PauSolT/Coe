using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Element : MonoBehaviour
{
    [field: SerializeField] ElementData data;
    public PassiveAbility Passive { get; private set; } = new();
    public List<ActiveAbility> Actives { get; private set; } = new();

    public Action<GameObject> OnElementEquip;
    public Action OnElementUnequip;

    public virtual void Init()
    {
        Log.Info($"{name} Init", LogCategory.Elements ,this);
        InitAbilityData();
        PopulateEvents();
    }

    private void OnDisable()
    {
        EraseEvents();
    }

    /// <summary>
    /// Prepares OnElementEquip and OnElementUnequip actions / events
    /// </summary>
    private void PopulateEvents()
    {
        OnElementEquip += Passive.Init;
        OnElementUnequip += Passive.Uninit;
        foreach (ActiveAbility aa in Actives)
        {
            OnElementEquip += aa.Init;
            OnElementUnequip += aa.Uninit;
        }
    }

    /// <summary>
    /// Deletes OnElementEquip and OnElementUnequip actions / events
    /// </summary>
    private void EraseEvents()
    {
        OnElementEquip -= Passive.Init;
        OnElementUnequip -= Passive.Uninit;
        foreach (ActiveAbility aa in Actives)
        {
            OnElementEquip -= aa.Init;
            OnElementUnequip -= aa.Uninit;
        }
    }
   

    /// <summary>
    /// Initializes all abilities data from ElementData
    /// Data from abilities have to be passed via inspector
    /// </summary>
    private void InitAbilityData()
    {
        Passive.SetData(data.abilityData[0]);
        for (int i = 1; i < data.abilityData.Count; i++)
        {
            Actives[i-1].SetData(data.abilityData[i]);
        }
    }

    /// <summary>
    /// Adds a new instance of a passive ability without the data
    /// </summary>
    /// <param name="passive">The instance of the new passive ability to add</param>
    protected void AddPassive(PassiveAbility passive)
    {
        Passive = passive;
    }

    /// <summary>
    /// Adds a new instance of an active ability without the data
    /// If there are 3 actives, it won't add to the active list
    /// </summary>
    /// <param name="active">The instance of the new active ability to add</param>
    protected void AddActive(ActiveAbility active)
    {
        if (Actives.Count < 3)
        {
            Actives.Add(active);
            Log.Info($"Ability {active}" +
                $"has been added to {data.elementName}", LogCategory.Elements);
        }
        else
        {
            Log.Info($"{data.elementName} already has max abilities");
        }
    }

}
