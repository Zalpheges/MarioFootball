using UnityEngine;
using UnityEngine.InputSystem;

public class InputBrain : PlayerBrain
{
    private Vector2 movementInput;
    public override Vector3 Move()
    {
        return new Vector3(-movementInput.y, 0, movementInput.x).normalized;
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        movementInput = input.ReadValue<Vector2>();
    }
}
