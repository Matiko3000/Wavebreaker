using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int MaxHealth = 50;
    [SerializeField] float destoryOnDeathDelay = 0.7f;
    [SerializeField] Collider2D mainCollider;

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
    Coroutine changingColors;
    Animator animator;


    private void Awake()
    {
        health = MaxHealth;
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();//make sure you find the sprite renderer and animator

        if (!isUsingAI)player = GetComponent<Player>(); //get the current component based on what the script is attached to
        else enemyAI = GetComponent<EnemyAI>();

        
    }

    #region takingDamage
    public void takeDamage(int damage, float kbDirection)//if used for player, overload with direction !!!
    {
        health -= damage;
        if (changingColors == null) changingColors = StartCoroutine(changeColors(sr.color, onDamageColor, colorDuration));
        player.ApplyKnockbackPlayer(kbDirection);

        Debug.Log(health);

        if (health <= 0) StartCoroutine(Die());
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if(changingColors == null)changingColors = StartCoroutine(changeColors(sr.color, onDamageColor, colorDuration));//fixing issues when player attacks same enemy twice before coroutine ends
        enemyAI.ApplyKnockbackEnemy();

        if (health <= 0 && isAlive) StartCoroutine(Die());
    }

    private IEnumerator changeColors(Color startColor, Color tempColor, float duration)
    {
        sr.color = tempColor;

        yield return new WaitForSeconds(duration);

        changingColors = null;
        sr.color = startColor;
    }
    #endregion

    IEnumerator Die()
    {
        isAlive = false;
        //if (mainCollider != null) mainCollider.enabled = false;//disable the collider ///QUESTIONABLE IF WORTH USING
        animator.SetFloat("velocity.y", 0); //make sure the animation play properly
        animator.SetTrigger("die");
        
        mainCollider.sharedMaterial = null;

        yield return new WaitForSeconds(destoryOnDeathDelay);//wait for the animation to play out

        Destroy(gameObject);
    }


        
}
