using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Element> elements;
    SpriteRenderer spriteRenderer;
    int currentElement = 0;
    PlayerUI playerUI;

    TimerManager timerManager;

    void Start()
    {
        //Get Components
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerUI = GetComponent<PlayerUI>();
        elements = new List<Element>(GetComponentsInChildren<Element>());
        timerManager = TimerManager.Instance;

        //Init Components
        playerUI.Init();
        foreach (Element element in elements)
        {
            element.Init();
            element.Passive.Init(gameObject, element.Data);
            element.Passive.SetImage(playerUI.AbilityImages()[0]);

            for (int i = 0; i < element.Actives.Count; i++)
            {
                ActiveAbility aa = element.Actives[i];
                aa.Init(gameObject, element.Data);
                aa.SetImage(playerUI.AbilityImages()[i + 1]);
            }
        }

        //Events
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
        timerManager.OnTimerUpdated += UpdateCurrentCooldowns;


        ChangePlayerColor();
    }

    private void OnDestroy()
    {
        foreach (Element element in elements)
        {
            element.Passive.Uninit(gameObject);
            foreach (ActiveAbility aa in element.Actives)
            {
                aa.Uninit(gameObject);
            }
        }

        timerManager.OnTimerUpdated -= UpdateCurrentCooldowns;
    }   

    /// <summary>
    /// Changes to previous element
    /// </summary>
    public void PreviousElement()
    {
        int previousElement = currentElement;
        elements[currentElement].OnElementUnequip?.Invoke(gameObject);
        currentElement--;
        if (currentElement < 0)
        {
            currentElement = elements.Count - 1;
        }
        ChangeElement();
        Log.Info($"Switched to previous element:{elements[currentElement].name}", LogCategory.Elements);
    }

    /// <summary>
    /// Changes to next element
    /// </summary>
    public void NextElement()
    {
        int previousElement = currentElement;
        elements[currentElement].OnElementUnequip?.Invoke(gameObject);
        currentElement++;
        if (currentElement >= elements.Count)
        {
            currentElement = 0;
        }
        ChangeElement();
        Log.Info($"Switched to next element:{elements[currentElement].name}", LogCategory.Elements);
    }

    /// <summary>
    /// Common chanign element functions
    /// </summary>
    private void ChangeElement()
    {
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
        ChangePlayerColor();
        UpdateCurrentCooldowns();
    }

    /// <summary>
    /// Function to call ability 1 in input
    /// </summary>
    public void UseAbility1()
    {
        UseAbility(0);
    }
    /// <summary>
    /// Function to call ability 2 in input
    /// </summary>
    public void UseAbility2()
    {
        UseAbility(1);
    }
    /// <summary>
    /// Function to call ability 3 in input
    /// </summary>
    public void UseAbility3()
    {
        UseAbility(2);
    }

    /// <summary>
    /// Logic for using an ability
    /// </summary>
    /// <param name="index">Index of the ability going from 0 to 2</param>
    private void UseAbility(int index)
    {
        List<ActiveAbility> actives = elements[currentElement].Actives;
        if (index < actives.Count)
        {
            actives[index].AbilityUse();
            Log.Info($"Pressed ability {index + 1} of element:{elements[currentElement].name}", LogCategory.Elements);
        }
    }

    /// <summary>
    /// Gets the specific existing element
    /// </summary>
    /// <typeparam name="T">Class of the element</typeparam>
    /// <returns>Returns the element</returns>
    public T GetSpecificElement<T>() where T : Element
    {
        foreach (Element item in elements)
        {
            if (item is T typedItem)
            {
                return typedItem;
            }
        }
        return null;
    }

    private void ChangePlayerColor()
    {
        Color color = GetCurrentElement().GetColor();
        spriteRenderer.color = color;
        GetCurrentElement().Passive.GetImage().color = color;
        foreach (ActiveAbility aa in GetCurrentElement().Actives)
        {
            aa.GetImage().color = color;
        }
    }

    /// <summary>
    /// Returns the current equipped element by the player
    /// </summary>
    /// <returns>Equipped element</returns>
    private Element GetCurrentElement()
    { 
        return elements[currentElement]; 
    }

    private void UpdateCurrentCooldowns()
    {
        PassiveAbility passive = GetCurrentElement().Passive;
        passive.UpdateCooldown(passive.GetCooldownPercentage());

        foreach (ActiveAbility aa in GetCurrentElement().Actives)
        {
            aa.UpdateCooldown(aa.GetCooldownPercentage());
        }
    }
}
