using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyCombat : MonoBehaviour
{
    Animator animator;
    EnemyAI enemyAI;

    [Header("Attack Stats")]
    [SerializeField] int baseDamage = 20;
    [SerializeField] float attackRate = 1.5f;
    [SerializeField] float attackRange = 0.5f;

    [Header("Visualisation")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float damageDelay = 0.2f;

    float nextAttackTime = 0f;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (Time.time < nextAttackTime) return;
        if (Physics2D.OverlapCircle(attackPoint.position, attackRange, LayerMask.GetMask("Player")) == null) return;// check if there is any player in range to attack
        StartCoroutine(Attack());
        nextAttackTime = Time.time + (1f / attackRate);
    }


    #region attacking
    IEnumerator Attack()
    {
        //play animatior
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(damageDelay);//wait for the animation so the attack looks natural
        //detect player in range
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("Player"));

        // Create a set to store unique players
        HashSet<GameObject> uniquePlayers = new HashSet<GameObject>();

        //damage them
        foreach (Collider2D playerCollider in hitPlayers)
        {
            GameObject player = playerCollider.gameObject;
            if (playerCollider.isTrigger == true) continue;
            if (uniquePlayers.Add(player)) // HashSet.Add returns false if the item was already in the set
            {
                player.GetComponent<Health>().takeDamage(baseDamage, enemyAI.getDirection());
            }
        }
    }
    #endregion

    //draw the range in editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}