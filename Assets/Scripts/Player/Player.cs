using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Element> elements;
    int currentElement = 0;
    void Start()
    {
        elements.AddRange(GetComponentsInChildren<Element>());
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
        Log.Info($"Switched to next element:{elements[currentElement].name}", LogCategory.Elements);
    }

    public void UseAbility1()
    {
        elements[currentElement].Actives[0].AbilityUse();
        Log.Info($"Used ability 1 of element:{elements[currentElement].name}", LogCategory.Elements);
    }
    public void UseAbility2()
    {
        elements[currentElement].Actives[1].AbilityUse();
        Log.Info($"Used ability 2 of element:{elements[currentElement].name}", LogCategory.Elements);
    }
    public void UseAbility3()
    {
        elements[currentElement].Actives[2].AbilityUse();
        Log.Info($"Used ability 3 of element:{elements[currentElement].name}", LogCategory.Elements);
    }

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
}
