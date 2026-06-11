using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public List<Element> elements;
    int currentElement = 0;
    void Start()
    {
        foreach (Element element in elements)
        {
            element.Init();
        }
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
        elements[currentElement].Actives[0].AbilityUse();
        elements[currentElement].Actives[1].AbilityUse();
    }

    void Update()
    {
        
    }   

    void PreviousElement()
    {
        elements[currentElement].OnElementUnequip?.Invoke();
        currentElement--;
        if (currentElement < 0)
        {
            currentElement = elements.Count - 1;
        }
        elements[currentElement].OnElementEquip?.Invoke(gameObject);

    }

    void NextElement()
    {
        elements[currentElement].OnElementUnequip?.Invoke();
        currentElement++;
        if (currentElement >= elements.Count)
        {
            currentElement = 0;
        }
        elements[currentElement].OnElementEquip?.Invoke(gameObject);
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
