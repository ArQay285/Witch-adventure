using UnityEngine;
using System.Collections;
using System;
using System.Runtime.CompilerServices;

[SelectionBase]

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }

    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [Header("Movement Settings")]
    [SerializeField] private float movingSpeed = 5f;
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float damageRecoveryTime = 0.7f;
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private TrailRenderer trailRenderer;


    private Rigidbody2D _rb;
    private KnockBack _knockBack;


    private float _initialMovingSpeed;
    private readonly float _minMovingSpeed = 0.1f;
    private bool _isRunning = false;
    private bool _CanDash = true;

    private int _currentHealth;
    private bool _canTakeDamage = true;
    private bool _isAlive = true;

    private Camera _mainCamera;

    Vector2 movement;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
        _mainCamera = Camera.main;
        _initialMovingSpeed = movingSpeed;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;

        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;

        _currentHealth = maxHealth;

    }
    private void Update()
    {
        movement = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (_knockBack.IsGettingKnockedBack)
            return;
        HandleMovement();
    }

    public bool IsAlive() => _isAlive;

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive) {
            _canTakeDamage = false;
            _currentHealth = Mathf.Max(0, _currentHealth -= damage);
            Debug.Log(_currentHealth);
            _knockBack.GetKnockedBack(damageSource);
            OnFlashBlink?.Invoke(this, EventArgs.Empty);
            StartCoroutine(DamageRecoveryCoroutine());
            Death();
        }
    }


    private IEnumerator DamageRecoveryCoroutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }

    private void Death()
    {
        if (_currentHealth <= 0 && _isAlive) {
            _isAlive = false;
            GameInput.Instance.DisableMovement();
            _knockBack.StopKnockBackMovement();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPlayerDash(object sender, System.EventArgs e)
    {
        Dash();
    }

    private void Dash()
    {
        if (_CanDash) {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        _CanDash = false;
        movingSpeed *= dashSpeed;
        trailRenderer.emitting = true;

        yield return new WaitForSeconds(dashDuration);
        trailRenderer.emitting = false;
        movingSpeed = _initialMovingSpeed;

        yield return new WaitForSeconds(dashCooldown);
        _CanDash = true;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }


    private void HandleMovement()
    {
        _rb.MovePosition(_rb.position + movement * (movingSpeed * Time.fixedDeltaTime));
        if (Mathf.Abs(movement.x) > _minMovingSpeed || Mathf.Abs(movement.y) > _minMovingSpeed) {
            _isRunning = true;
        }
        else {
            _isRunning = false;
        }
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
    }
}


