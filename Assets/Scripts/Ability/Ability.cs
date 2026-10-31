using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Ability
{
    [SerializeField] protected AbilityData data;
    protected ElementData elementData;
    protected Image abilityImage;

    protected bool onCooldown = false;
    protected Timer cooldownTimer = null;

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

    public void SetImage(Image image)
    {
        abilityImage = image;
    }

    public Image GetImage()
    {
        return abilityImage;
    }

    public void UpdateCooldown(float percentage)
    {
        if (abilityImage != null)
        {
            abilityImage.fillAmount = percentage;
        }
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
    /// Returns the percentage of the cooldown from 0 (just used the ability) to 1 (can use the ability)
    /// </summary>
    /// <returns>Float between 0 to 1</returns>
    public float GetCooldownPercentage()
    {
        if (cooldownTimer == null)
        {
            return 1f;
        }
        else
        {
            return cooldownTimer.ProgressAscending;
        }
    }

    /// <summary>
    /// Returns the seconds of the cooldown from 0 (can use the ability) to the ability's cooldown (just used the ability)
    /// </summary>
    /// <returns>Float between 0 to ability's cooldown</returns>
    public float GetCooldownSecondsRemaining()
    {
        if (cooldownTimer == null)
        {
            return 0f;
        }
        else
        {
            return cooldownTimer.CurrentTime;
        }
    }
}
