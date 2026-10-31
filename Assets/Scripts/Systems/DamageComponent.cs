using System;

/// <summary>
/// Info about the damage
/// </summary>
[Serializable]
public class DamageComponent
{
    private float amount;
    private float multiplier = 1;
    public ElementData ElementData { get; private set; }

    public bool HitFromRight { get; private set; }

    public DamageComponent(float amount, ElementData elementData, float multiplier)
    {
        this.amount = amount;
        this.multiplier = multiplier;
        ElementData = elementData;
    }

    public void AddMultiplier(float mult)
    {
        multiplier += mult;
    }

    public void SetMultiplier(float mult)
    {
        multiplier = mult;
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
    }

    public float GetTotalAmount()
    {
        return amount * multiplier;
    }

    public void SetHitFromRight(bool hitFromRight)
    {
        HitFromRight = hitFromRight;
    }
}