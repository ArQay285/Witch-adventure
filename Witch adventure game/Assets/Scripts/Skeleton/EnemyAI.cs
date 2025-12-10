using UnityEngine;
using UnityEngine.AI;
using TopDownGame.Utils;
using UnityEngine.EventSystems;
using System;


public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;

    [SerializeField] private bool isChasingEnemy = false;
    [SerializeField] private float chasingDistance = 5f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;

    [SerializeField] private bool isAttackingEnemy = false;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float attackingDistance = 1f;

    public event EventHandler OnEnemyAttack;

    private float _nextAttackTime = 0f;

    private NavMeshAgent _navMeshAgent;

    private State _currentState;
    private float _roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public bool IsRunning => _navMeshAgent.velocity != Vector3.zero;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = startingState;
        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _roamingSpeed * chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
    }

    public void SetDeathState()
    {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    private void StateHandler()
    {
        switch (_currentState) {
            case State.Roaming:
            _roamingTimer -= Time.deltaTime;
            if (_roamingTimer < 0) {
                Roaming();
                _roamingTimer = roamingTimerMax;
            }
            CheckCurrentState();
            break;
            case State.Chasing:
            ChasingTarget();
            CheckCurrentState();
            break;
            case State.Attacking:
            AttakingTarget();
            CheckCurrentState();
            break;
            case State.Death:
            break;
            default:
            case State.Idle:
            break;
        }
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (isChasingEnemy && Player.Instance.IsAlive()) {
            if (distanceToPlayer < chasingDistance) {
                newState = State.Chasing;
            }
        }

        if (isAttackingEnemy && Player.Instance.IsAlive()) {
            if (distanceToPlayer < attackingDistance) {
                newState = State.Attacking;
            }
        }

        if (newState != _currentState) {
            if (newState == State.Chasing) {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.Roaming) {
                _navMeshAgent.speed = _roamingSpeed;
                _roamingTimer = roamingTimerMax;
            }
            else if (newState == State.Attacking) {
                _navMeshAgent.ResetPath();
            }

            _currentState = newState;
        }
    }

    private void AttakingTarget()
    {
        if (Time.time >= _nextAttackTime) {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + attackRate;
        }
    }

    private void ChasingTarget()
    {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void Roaming()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + GameUtils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void changeFactionDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x) {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time >= _nextCheckDirectionTime) {
            if (IsRunning) {
                changeFactionDirection(_lastPosition, transform.position);
            }
            else if (_currentState == State.Attacking) {
                changeFactionDirection(transform.position, Player.Instance.transform.position);
            }
            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }
}


