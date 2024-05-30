using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Collision behaviour")]
    [SerializeField] bool dealDamageOnCollision = false;
    [SerializeField] int damage = 10;
    [SerializeField] Vector2 knockbackForce = new Vector2(3, 4);
    [SerializeField] float knockbackDuration = 0.5f;



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
        collision.gameObject.GetComponent<Health>().takeDamage(damage);
        Knockback();
    }
    void Knockback()
    {
        float direction = -EnemyAI.GetDirection();
        Vector2 force = new Vector2(direction * knockbackForce.x, knockbackForce.y);

        EnemyAI.ApplyKnockback(force, knockbackDuration);
    }
}
