using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamageDealer : MonoBehaviour
{
    [Header("Collision behaviour")]
    [SerializeField] bool dealDamageOnCollision = false;
    [SerializeField] int damage = 10;

    EnemyAI EnemyAI;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        EnemyAI = GetComponent<EnemyAI>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || !dealDamageOnCollision) return;
        collision.gameObject.GetComponent<Health>().takeDamage(damage, transform.localScale.x); //uses the localscale.x so the player gets knock in opposite direction from the enemy no matter which way they are facing
        EnemyAI.ApplyKnockbackEnemy();//applaies knockback for itself
    }
}
