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

    [HideInInspector] public bool isAlive = true;

    public int health;

    SpriteRenderer sr;
    Player player;



    private void Awake()
    {
        health = MaxHealth;
        isAlive = true;
        sr = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(changeColors(sr.color, onDamageColor, colorDuration));
        player.ApplyKnockback();

        Debug.Log(health);

        if (health <= 0) Die();
    }

    void Die()
    {
        isAlive = false;
        Destroy(gameObject);
    }

    private IEnumerator changeColors(Color startColor, Color tempColor, float duration)
    {
        sr.color = tempColor;

        yield return new WaitForSeconds(duration);

        sr.color = startColor;
    }
        
}
