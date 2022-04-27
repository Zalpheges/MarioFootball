using UnityEngine.InputSystem;
using UnityEngine;

public class InputBrain : PlayerBrain
{
    private Vector3 _movementInput;
    private Vector3 _rightStickInput;

    private float _shootForce;

    private bool _leftTrigger = false;
    private bool _westButtonPressed = false;
    private bool _westButtonHeld = false;
    private bool _eastButton = false;
    private bool _northButton = false;
    private bool _southButton = false;

    private const float _loadTime = 1.5f;

    /// <summary>
    /// Compute the displacement applied to the player by the device
    /// </summary>
    /// <param name="team">The player's team</param>
    /// <returns>The displacement vector.</returns>
    /// 
    private void Start()
    {
        _shootForce = 0;
    }
    private void Update()
    {

    }

    //-------------------------------------------------------Revoir tout les return de GetAction() !!!!
    public override Action GetAction()
    {
        if (_westButtonPressed) // Shoot|Tackle
        {
            _westButtonPressed = false;

            if (Player.HasBall)
            {
                return Action.Shoot(0f);
            }
            else
                return Action.Tackle(_movementInput);
        }
        else if (_eastButton) // Throw
        {
            _eastButton = false;
            return Action.Throw(_movementInput);
        }
        else if (_southButton) // ChangePlayer/Pass
        {
            _southButton = false;

            if (Player.HasBall)
            {
                if (_leftTrigger)
                    return Action.LobPass(_movementInput); // With bezier point
                else
                    return Action.Pass(_movementInput); // Without bezier point
            }
            else
                return Action.ChangePlayer();

        }
        else if (_northButton) // Drible/HeadButt
        {
            _northButton = false;

            if (Player.HasBall)
                return Action.Dribble();
            else
                return Action.Headbutt(_movementInput);
        }
        else
        {
            if (_westButtonHeld)
                return Action.Loading(1f);// shootForce / loadTime
            else
            {
                if (_movementInput != Vector3.zero)
                    return Action.Move(_movementInput);
                else
                    return Action.None;
            }
        }
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
        if (context.phase == InputActionPhase.Performed)
        {
            _westButtonPressed = true;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            //_westButtonHeld = false;
            //_westButtonPressed = true;
        }
    }

    public void EastButton(InputAction.CallbackContext context)
    {

        if (context.phase == InputActionPhase.Performed)
            _eastButton = true;
    }

    public void NorthButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            _northButton = true;
    }

    public void SouthButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            _southButton = true;
    }

    public void LeftTrigger(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            _leftTrigger = true;
        if (context.phase == InputActionPhase.Canceled)
            _leftTrigger = false;
    }
}
