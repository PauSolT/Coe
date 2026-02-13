using UnityEngine;

public class Ability
{
    [field: SerializeField] protected AbilityData data;

    /// <summary>
    /// Initialize ability when swapping in the element.
    /// </summary>
    public virtual void Init(GameObject owner)
    {

    }

    /// <summary>
    /// Uninitialize ability when swapping out the element.
    /// </summary>
    public virtual void Uninit()
    {

    }

    public void SetData(AbilityData data)
    { 
        this.data = data; 
    }
    public AbilityData GetData()
    {
        return data;
    }
}
