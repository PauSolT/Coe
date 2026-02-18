using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] InputReader inputReader;
    PlayerController controller;

    float moveSpeed = 6;
    float gravity = -20;
    Vector3 velocity;
    Vector2 input;

    public List<Element> elements;
    int currentElement = 0;
    void Start()
    {
        //foreach (Element element in elements)
        //{
        //    element.Init();
        //}
        //elements[currentElement].OnElementEquip?.Invoke(gameObject);
        //elements[currentElement].Actives[0].AbilityUse();

        inputReader.MoveEvent += Move;
        inputReader.JumpEvent += Jump;
        inputReader.CancelJumpEvent += CancelJump;

        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        velocity.x = input.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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


    void Move(Vector2 input)
    {
        this.input = input;
        Log.Info($"Player moving: {input}", this);
    }

    void Jump()
    {
        Log.Info($"Player jumped", this);
    }

    void CancelJump()
    {
        Log.Info($"Player canceled jumped", this);
    }
}
