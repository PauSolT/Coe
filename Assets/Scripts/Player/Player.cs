using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Element> elements;
    SpriteRenderer spriteRenderer;
    int currentElement = 0;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        elements = new List<Element>(GetComponentsInChildren<Element>());
        foreach (Element element in elements)
        {
            element.Init();
        }
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
    }

    void Update()
    {
        
    }   

    public void PreviousElement()
    {

        elements[currentElement].OnElementUnequip?.Invoke();
        currentElement--;
        if (currentElement < 0)
        {
            currentElement = elements.Count - 1;
        }
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
        ChangePlayerColor();
        Log.Info($"Switched to previous element:{elements[currentElement].name}", LogCategory.Elements);
    }

    public void NextElement()
    {
        elements[currentElement].OnElementUnequip?.Invoke();
        currentElement++;
        if (currentElement >= elements.Count)
        {
            currentElement = 0;
        }
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
        ChangePlayerColor();
        Log.Info($"Switched to next element:{elements[currentElement].name}", LogCategory.Elements);
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
        spriteRenderer.color = elements[currentElement].GetColor();
    }
}
