using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int MaxHealth = 50;

    [Header("Colors on damage")]
    [SerializeField] Color onDamageColor = Color.red;
    [SerializeField] float colorDuration = 0.4f;

    [Header("AI")]
    [SerializeField] bool isUsingAI = false;

    [HideInInspector] public bool isAlive = true;

    public int health;

    SpriteRenderer sr;
    Player player;
    EnemyAI enemyAI;


    private void Awake()
    {
        health = MaxHealth;
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        if(!isUsingAI)player = GetComponent<Player>(); //get the current component based on what the script is attached to
        else enemyAI = GetComponent<EnemyAI>();
    }

    #region takingDamage
    public void takeDamage(int damage, float kbDirection)//if used for player, overload with direction !!!
    {
        health -= damage;
        StartCoroutine(changeColors(sr.color, onDamageColor, colorDuration));
        player.ApplyKnockbackPlayer(kbDirection);

        Debug.Log(health);

        if (health <= 0) Die();
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(changeColors(sr.color, onDamageColor, colorDuration));
        enemyAI.ApplyKnockbackEnemy();

        if (health <= 0) Die();
    }

    private IEnumerator changeColors(Color startColor, Color tempColor, float duration)
    {
        sr.color = tempColor;

        yield return new WaitForSeconds(duration);

        sr.color = startColor;
    }
    #endregion

    void Die()
    {
        isAlive = false;
        Destroy(gameObject);
    }


        
}
