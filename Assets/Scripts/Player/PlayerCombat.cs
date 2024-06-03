using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    Animator animator;
    Player player;

    [Header("Attack Stats")]
    [SerializeField] int baseDamage = 20;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float attackRange = 0.5f;

    [SerializeField] float attackSlowRatio = 0.5f;
    [SerializeField] float attackSlowDuration = 0.5f;

    [Header("Visualisation")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float damageDelay = 0.2f;

    float nextAttackTime = 0f;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    void OnAttack(InputValue value)
    {
        if (Time.time < nextAttackTime) return;
        StartCoroutine(Attack());
        nextAttackTime = Time.time + (1f / attackRate);
    }

    IEnumerator Attack()
    {
        //play animatior and slow the player
        animator.SetTrigger("attack");
        player.SlowPlayer(attackSlowDuration, attackSlowRatio);

        yield return new WaitForSeconds(damageDelay);//wait for the animation so the attack looks natural
        //detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("Enemy"));

        // Create a set to store unique enemies
        HashSet<GameObject> uniqueEnemies = new HashSet<GameObject>();

        //damage them
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            GameObject enemy = enemyCollider.gameObject;
            if (uniqueEnemies.Add(enemy)) // HashSet.Add returns false if the item was already in the set
            {
                Debug.Log(enemy.name);
                enemy.GetComponent<Health>().takeDamage(baseDamage);
            }
        }
    }

    //draw the range in editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
