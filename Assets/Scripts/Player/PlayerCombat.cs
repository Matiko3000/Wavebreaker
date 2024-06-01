using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    Animator animator;
    Player player;

    [SerializeField] float attackSlowRatio = 0.5f;
    [SerializeField] float attackSlowDuration = 0.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }
    void Update()
    {
        
    }

    void OnAttack(InputValue value)
    {
        Attack();
    }

    void Attack()
    {
        animator.SetTrigger("attack");
        player.SlowPlayer(attackSlowDuration, attackSlowRatio);
    }
}
