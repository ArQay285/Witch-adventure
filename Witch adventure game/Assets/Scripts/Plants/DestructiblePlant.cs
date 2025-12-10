using System;
using UnityEngine;
using UnityEngine.AI;

public class DestructiblePlant : MonoBehaviour
{

    public event EventHandler OnDestructibleTakeDamage;

    private NavMeshObstacle obstacle;

    private void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null) {
            obstacle.carving = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>()) {
            OnDestructibleTakeDamage?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
}
