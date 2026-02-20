using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] InputReader inputReader;
    Controller2D controller;

    [field: SerializeField] float jumpHeight = 4;
    [field: SerializeField] float timeToJumpApex = 0.4f;
    [field: SerializeField] float moveSpeed = 6;

    float accelerationTimeAirbone = 0.2f;
    float accelerationTimeGrounded = 0.1f;

    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    Vector3 velocity;
    Vector2 input;

    public List<Element> elements;
    int currentElement = 0;

    bool pressedJump = false;
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

        controller = GetComponent<Controller2D>();

        //Formula to get gravity, knowing jump height and time to jump to apex point
        //Negative is to make gravity negative
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //Formula to get jump velocity, with already calculated gravity
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
    }

    void FixedUpdate()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        if (pressedJump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirbone));
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
        pressedJump = true;

        Log.Info($"Player jumped", this);
    }

    void CancelJump()
    {
        pressedJump = false;
        Log.Info($"Player canceled jumped", this);
    }
}
