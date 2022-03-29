using UnityEngine.InputSystem;
using UnityEngine;

public class InputBrain : PlayerBrain
{
    private Vector3 _movementInput;
    private Vector3 _rightStickInput;

    private float shootForce;

    private bool leftTrigger = false;
    private bool westButtonPressed = false;
    private bool westButtonHeld = false;
    private bool eastButton = false;
    private bool northButton = false;
    private bool southButton = false;

    /// <summary>
    /// Calcule le d�placement que la manette applique au joueur 
    /// </summary>
    /// <param name="team">L'�quipe du joueur</param>
    /// <returns>Le vecteur de d�placement.</returns>
    /// 
    private void Update()
    {
        if (westButtonHeld)
        {
            shootForce += Time.deltaTime; // Force en fonction du ntemp appuyé A REVOIR
        }
    }
    //-------------------------------------------------------Revoir tout les return de GetAction() !!!!
    public override Action GetAction()
    {
        if (westButtonPressed) // Tire|Tacle
        {
            westButtonPressed = !westButtonPressed;

            if (Player.HasBall)
            {
                return Action.Shoot(_rightStickInput, shootForce);
            }
            else
                return Action.Tackle(_rightStickInput);
        }
        else if (eastButton) //Throw
        {
            eastButton = !eastButton;
            return Action.Throw(_rightStickInput);
        }
        else if (southButton) // ChangePlayer/Pass
        {
            southButton = !southButton;

            if (Player.HasBall)
            {
                if (leftTrigger)
                    return Action.LobPass(_rightStickInput); // With bezier point
                else
                    return Action.Pass(_rightStickInput); // Without bezier point
            }
            else
                return Action.ChangePlayer();

        }
        else if (northButton) // Drible/HeadBut
        {
            northButton = !northButton;

            if (Player.HasBall)
                return Action.Dribble();
            else
                return Action.Headbutt(_rightStickInput);
        }
        else if (_movementInput != Vector3.zero)
            return Action.Move(_movementInput);
        else
            return Action.None;
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        _movementInput = input.ReadValue<Vector2>();
        _movementInput = new Vector3(_movementInput.x, 0f, _movementInput.y);
    }

    public void OnRightStick(InputAction.CallbackContext input)
    {
        _rightStickInput = input.ReadValue<Vector2>();
        _rightStickInput = new Vector3(_rightStickInput.x, 0f, _rightStickInput.y);
    }

    public void WestButton(InputAction.CallbackContext context)
    {
        shootForce = 10;

        if (context.phase == InputActionPhase.Performed)
        {
            westButtonHeld = true;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            westButtonHeld = false;
            westButtonPressed = true;
        }
    }

    public void EastButton(InputAction.CallbackContext context)
    {

        if (context.phase == InputActionPhase.Performed)
            eastButton = !eastButton;
    }

    public void NorthButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            northButton = !northButton;
    }

    public void SouthButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            southButton = !southButton;
    }

    public void LeftTrigger(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            leftTrigger = true;
        if (context.phase == InputActionPhase.Canceled)
            leftTrigger = false;
    }
}
