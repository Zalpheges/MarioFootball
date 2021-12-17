using UnityEngine;
using UnityEngine.InputSystem;

public class InputBrain : PlayerBrain
{
    private float speed = 2;
    private Vector2 movementInput;
    /// <summary>
    /// Calcule le déplacement que la manette applique au joueur 
    /// </summary>
    /// <param name="team">L'équipe du joueur</param>
    /// <returns>Le vecteur de déplacement.</returns>
    public override Vector2 Move(Team team)
    {
        if (movementInput.x != 0 || movementInput.y != 0)
        {
            return new Vector3(movementInput.x, 0, movementInput.y).normalized;

            //rb.MovePosition(rb.position + direction * speed * Time.deltaTime);

            //target = Quaternion.LookRotation(direction, Vector3.up);
        }
        //rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, Time.deltaTime * 10));

        return Vector3.zero;
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        movementInput = input.ReadValue<Vector2>();
    }
}
