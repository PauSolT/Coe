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

    public void UseAbility1()
    {
        Log.Info($"Pressed ability 1 of element:{elements[currentElement].name}", LogCategory.Elements);
        elements[currentElement].Actives[0].AbilityUse();
    }
    public void UseAbility2()
    {
        Log.Info($"Pressed ability 2 of element:{elements[currentElement].name}", LogCategory.Elements);
        elements[currentElement].Actives[1].AbilityUse();
    }
    public void UseAbility3()
    {
        Log.Info($"Pressed ability 3 of element:{elements[currentElement].name}", LogCategory.Elements);
        elements[currentElement].Actives[2].AbilityUse();
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

    private void ChangePlayerColor()
    {
        spriteRenderer.color = elements[currentElement].GetColor();
    }
}
