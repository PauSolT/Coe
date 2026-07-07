using UnityEngine;

public class Ability
{
    [SerializeField] protected AbilityData data;
    protected ElementData elementData;

    /// <summary>
    /// Initialize ability when swapping in the element.
    /// </summary>
    public virtual void Init(GameObject owner, ElementData elementData)
    {
        this.elementData = elementData;
    }

    /// <summary>
    /// Uninitialize ability when swapping out the element.
    /// </summary>
    public virtual void Uninit(GameObject owner)
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
