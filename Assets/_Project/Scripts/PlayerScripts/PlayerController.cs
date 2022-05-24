using UnityEngine;

[RequireComponent(typeof(InputBuffer))]
public class PlayerController : BaseController
{
    private bool _hasJumped;
    private bool reset;
    private bool k;
    private bool j;
    private float _dashInputCooldown;
    private bool reset2;
    private bool k2;
    private bool j2;
    private float _dashInputCooldown2;

    public Vector2 InputDirection { get; private set; }


    void Update()
    {
        if (!string.IsNullOrEmpty(_brainController.ControllerInputName))
        {
            if (IsControllerEnabled)
            {
                Movement();
                Jump();
                Crouch();
                Light();
                Medium();
                Heavy();
                Arcane();
                Assist();
                Throw();
                Dash();
            }
            Pause();
            ResetRound();
            SwitchCharacter();
        }
    }

    protected virtual void Movement()
    {
        InputDirection = new(Input.GetAxisRaw(_brainController.ControllerInputName + "Horizontal"), Input.GetAxisRaw(_brainController.ControllerInputName + "Vertical"));
        if (InputDirection.x == 1.0f && _playerMovement.MovementInput.x != InputDirection.x)
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Direction, InputDirectionEnum.Right);
        }
        if (InputDirection.x == -1.0f && _playerMovement.MovementInput.x != InputDirection.x)
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Direction, InputDirectionEnum.Left);
        }
        if (InputDirection.y == 1.0f && _playerMovement.MovementInput.y != InputDirection.y)
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Direction, InputDirectionEnum.Up);
        }
        if (InputDirection.y == -1.0f && _playerMovement.MovementInput.y != InputDirection.y)
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Direction, InputDirectionEnum.Down);
        }
        _playerMovement.MovementInput = InputDirection;
    }

    public virtual bool Jump()
    {
        if (InputDirection.y > 0.0f && !_hasJumped)
        {
            _hasJumped = true;
            return true;
        }
        else
        {
            _hasJumped = false;
            return false;
        }
    }

    public virtual bool Crouch()
    {
        if (InputDirection.y < 0.0f)
        {
            return true;
        }
        return false;
    }

    public virtual bool StandUp()
    {
        if (InputDirection.y == 0.0f)
        {
            return true;
        }
        return false;
    }

    protected virtual void Light()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Light"))
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Light);
        }
    }

    protected virtual void Medium()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Medium"))
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Medium);
        }
    }

    protected virtual void Heavy()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Heavy"))
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Heavy);
        }
    }

    protected virtual void Arcane()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Arcane"))
        {
            _inputBuffer.AddInputBufferItem(InputEnum.Special);
        }
    }

    protected virtual void Assist()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Assist"))
        {
            _player.AssistAction();
            _inputBuffer.AddInputBufferItem(InputEnum.Assist);
        }
    }

    protected virtual void Throw()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Throw"))
        {
            _playerStateManager.TryToThrowState();
            _inputBuffer.AddInputBufferItem(InputEnum.Throw);
        }
    }

    private void ResetRound()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Reset"))
        {
            GameManager.Instance.ResetRound(_playerMovement.MovementInput);
        }
    }

    private void SwitchCharacter()
    {
        if (Input.GetButtonDown(_brainController.ControllerInputName + "Switch"))
        {
            GameManager.Instance.SwitchCharacters();
        }
    }

    private void Pause()
    {
        if (GameManager.Instance.HasGameStarted)
        {
            if (Input.GetButtonDown(_brainController.ControllerInputName + "Pause"))
            {
                _player.Pause(_brainController.IsPlayerOne);
            }
            if (Input.GetButtonUp(_brainController.ControllerInputName + "Pause"))
            {
                _player.UnPause();
            }
        }
    }

    protected virtual void Dash()
    {
        // DoubleTapAxis(1);
        // DoubleTapAxis2(-1);
    }

    public bool DashForward()
    {
        float input = Input.GetAxisRaw(_brainController.ControllerInputName + "Horizontal");
        if (input == 1)
        {
            if (_dashInputCooldown > 0 && k)
            {
                if (!j)
                {
                    j = true;
                    return true;
                }
            }
            else
            {
                _dashInputCooldown = 0.15f;
                reset = true;
            }
        }
        else if (Input.GetAxisRaw(_brainController.ControllerInputName + "Horizontal") == 0.0f && reset)
        {
            k = true;
            if (j)
            {
                reset = false;
                k = false;
                j = false;
            }
        }

        if (_dashInputCooldown > 0)
        {
            _dashInputCooldown -= 1 * Time.deltaTime;
        }
        else
        {
            reset = false;
            k = false;
            j = false;
        }
        return false;
    }

    public bool DashBackward()
    {
        float input = Input.GetAxisRaw(_brainController.ControllerInputName + "Horizontal");
        if (input == -1)
        {
            if (_dashInputCooldown2 > 0 && k2)
            {
                if (!j2)
                {
                    j2 = true;
                    return true;
                }
            }
            else
            {
                _dashInputCooldown2 = 0.15f;
                reset2 = true;
            }
        }
        else if (Input.GetAxisRaw(_brainController.ControllerInputName + "Horizontal") == 0.0f && reset2)
        {
            k2 = true;
            if (j2)
            {
                reset2 = false;
                k2 = false;
                j2 = false;
            }
        }

        if (_dashInputCooldown2 > 0)
        {
            _dashInputCooldown2 -= 1 * Time.deltaTime;
        }
        else
        {
            reset2 = false;
            k2 = false;
            j2 = false;
        }
        return false;
    }
}
