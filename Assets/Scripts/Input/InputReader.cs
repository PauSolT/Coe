using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = nameof(InputReader), menuName = "SO/" + nameof(InputReader))]
public class InputReader : ScriptableObject, InputActions.IPlayerActions
{
    //Holds all the player inputs
    private InputActions inputs;

    //Create an event for every input
    public event Action<Vector2> MoveEvent;
    public event Action JumpEvent;
    public event Action CancelJumpEvent;
    public event Action Ability1Event;
    public event Action Ability2Event;
    public event Action Ability3Event;
    public event Action PreviousElementEvent;
    public event Action NextElementEvent;



    private void OnEnable()
    {
        if (inputs == null)
        {
            inputs = new InputActions();
            //Tells the input system to send the player action callbacks to this scriptableObject
            inputs.Player.SetCallbacks(this);
        }
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs?.Disable();
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
    }

    public void OnCancelJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            CancelJumpEvent?.Invoke();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnAbility1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Ability1Event?.Invoke();
        }
    }

    public void OnAbility2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Ability2Event?.Invoke();
        }
    }

    public void OnAbility3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Ability3Event?.Invoke();
        }
    }

    public void OnPreviousElement(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PreviousElementEvent?.Invoke();
        }
    }

    public void OnNextElement(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            NextElementEvent?.Invoke();
        }
    }
}
