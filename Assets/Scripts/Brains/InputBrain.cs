using UnityEngine.InputSystem;
using UnityEngine;

public class InputBrain : PlayerBrain
{
    private Vector2 _movementInput;
    private Vector2 _rightStickInput;

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
        if(westButtonHeld)
        {
            shootForce += Time.deltaTime; // Force en fonction du ntemp appuyé A REVOIR
        }
    }
    //-------------------------------------------------------Revoir tout les return de GetAction() !!!!
    public override Action GetAction()
    {
        if(westButtonPressed) // Tire|Tacle
        {
            westButtonPressed = false;

            if (true)
            {
                return Action.Shoot(shootForce, _rightStickInput, Vector3.zero, 5);
            }
                
            else
                return Action.Tackle(_rightStickInput);
        }
        else if(eastButton) //Throw
        {
            return Action.Throw(_rightStickInput);
        }
        else if(southButton) // ChangePlayer/Pass
        {
            if (true)
            {
                if (leftTrigger)
                {
                    return Action.Pass(_rightStickInput, Vector3.zero, Vector3.zero, Vector3.zero, 2); // With bezier point
                }
                else return Action.Pass(_rightStickInput, Vector3.zero, Vector3.zero, 2); // Without bezier point
            }
            else
                return Action.ChangePlayer();
            
        }
        else if(northButton) // Drible/HeadBut
        {
            if (true)
                return Action.Dribble();
            else
                return Action.Headbutt(_rightStickInput);
        }
        else 
        {
            return Action.Move(new Vector3(_movementInput.x, 0, _movementInput.y).normalized);
        }
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        _movementInput = input.ReadValue<Vector2>();
    }

    public void OnRightStick(InputAction.CallbackContext input)
    {
        _rightStickInput = input.ReadValue<Vector2>();
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

        if(context.phase== InputActionPhase.Started || context.phase == InputActionPhase.Canceled)
            eastButton = !eastButton;
    }

    public void NorthButton()
    {
            northButton = !northButton;
    }

    public void SouthButton()
    {
            southButton = !southButton;
    }

    public void LeftTrigger()
    {
            leftTrigger = !leftTrigger;
    }
}
