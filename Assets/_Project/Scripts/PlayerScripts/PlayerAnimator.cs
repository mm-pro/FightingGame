using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats = default;
    [SerializeField] private InputBuffer _inputBuffer = default;
    private Animator _animator;
    private SpriteLibrary _spriteLibrary;
    private SpriteRenderer _spriteRenderer;

    public PlayerStats PlayerStats { get { return _playerStats; } private set { } }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteLibrary = GetComponent<SpriteLibrary>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _animator.runtimeAnimatorController = _playerStats.PlayerStatsSO.runtimeAnimatorController;
    }

    public void Walk()
    {
        _animator.Play("Walk");
    }

    public void Idle()
    {
        _animator.Play("Idle");
    }

    public void Crouch()
    {
        _animator.Play("Crouch");
    }

    public void Jump(bool reset = false)
    {
        if (reset)
        {
            _animator.Play("Jump", -1, 0f);
        }
        else
        {
            _animator.Play("Jump");
        }
    }

    public void JumpForward(bool reset = false)
    {
        if (reset)
        {
            _animator.Play("JumpForward", -1, 0f);
        }
        else
        {
            _animator.Play("JumpForward");
        }
    }

    public void CancelAttack()
    {
        _animator.SetTrigger("Cancel");
    }

    public void CancelHurt()
    {
        _animator.SetTrigger("CancelHurt");
    }

    public void ResetAnimation(string name)
    {
        _animator.Play(name, -1, 0f);
    }

    public void ResetTrigger(string name)
    {
        _animator.ResetTrigger(name);
    }

    public void Attack(string attackType, bool reset = false)
    {
        if (reset)
        {
            _animator.Play(attackType, -1, 0f);
        }
        else
        {
            _animator.Play(attackType);
        }
    }

    public void Shadowbreak()
    {
        _animator.SetTrigger("Shadowbreak");
    }

    public void Intro()
    {
        _animator.Play("Intro");
    }

    public void Throw()
    {
        _animator.Play("Throw");
    }

    public void ThrowEnd()
    {
        _animator.SetTrigger("ThrowEnd");
    }

    public void Arcana()
    {
        _animator.Play("Arcana");
    }

    public void ArcanaEnd()
    {
        _animator.SetTrigger("ArcanaEnd");
    }

    public void Hurt(bool reset = false)
    {
        if (reset)
        {
            _animator.Play("Hurt", -1, 0f);
        }
        else
        {
            _animator.Play("Hurt");
        }
    }

    public void IsBlocking(bool state)
    {
        _animator.SetBool("IsBlocking", state);
    }

    public void IsBlockingLow(bool state)
    {
        _animator.SetBool("IsBlockingLow", state);
    }
    public void IsBlockingAir(bool state)
    {
        _animator.SetBool("IsBlockingAir", state);
    }

    public void Dash()
    {
        _animator.Play("Dash");
    }

    public void AirDash()
    {
        _animator.Play("Jump");
    }

    public void Run()
    {
        _animator.Play("Run");
    }

    public void Taunt()
    {
        _animator.SetTrigger("Taunt");
    }

    public void Death()
    {
        _animator.SetTrigger("Death");
    }

    public void IsKnockedDown(bool state)
    {
        _animator.SetBool("IsKnockedDown", state);
    }

    public void Rebind()
    {
        _animator.Rebind();
    }

    public Sprite GetCurrentSprite()
    {
        return _spriteRenderer.sprite;
    }

    public int SetSpriteLibraryAsset(int skinNumber)
    {
        if (skinNumber > PlayerStats.PlayerStatsSO.spriteLibraryAssets.Length - 1)
        {
            skinNumber = 0;
        }
        else if (skinNumber < 0)
        {
            skinNumber = PlayerStats.PlayerStatsSO.spriteLibraryAssets.Length - 1;
        }
        _spriteLibrary.spriteLibraryAsset = PlayerStats.PlayerStatsSO.spriteLibraryAssets[skinNumber];
        return skinNumber;
    }

    public void SetSpriteOrder(int index)
    {
        _spriteRenderer.sortingOrder = index;
    }
}
