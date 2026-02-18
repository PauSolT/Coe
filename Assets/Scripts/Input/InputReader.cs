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
}
