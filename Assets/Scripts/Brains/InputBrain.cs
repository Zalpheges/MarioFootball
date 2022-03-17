using UnityEngine.InputSystem;
using UnityEngine;

public class InputBrain : PlayerBrain
{
    private Vector2 _movementInput;

    /// <summary>
    /// Calcule le d�placement que la manette applique au joueur 
    /// </summary>
    /// <param name="team">L'�quipe du joueur</param>
    /// <returns>Le vecteur de d�placement.</returns>
    public override Action GetAction()
    {
        return Action.Move(new Vector3(_movementInput.x, 0, _movementInput.y).normalized);
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        _movementInput = input.ReadValue<Vector2>();
    }
}
