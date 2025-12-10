using UnityEngine;
using System;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{

    [SerializeField] private EnemySO enemySO;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private int _currentHealth;


    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        _currentHealth = enemySO.enemyHealth;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<Player>(out Player player)) {
            player.TakeDamage(transform, enemySO.enemyDamageAmount);
        }
    }

    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }

    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        if (_currentHealth <= 0)
            Death();
    }

    private void Death()
    {
        _boxCollider2D.enabled = false;
        _polygonCollider2D.enabled = false;
        _enemyAI.SetDeathState();
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

}
