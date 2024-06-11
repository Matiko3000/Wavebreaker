using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.WSA;
using UnityEditor;
using Cinemachine.Utility;
using System;
using UnityEngine.Windows;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] GameObject target;
    [SerializeField] float activateDistance = 50f;
    [SerializeField] float pathUpdateSeconds = 0.5f;
    [SerializeField] int pathPredictScale = 4;//can stop working if too high

    [Header("Physics")]
    [SerializeField] float speed = 10f;
    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float jumpNodeHeightRequirement = 1f;
    [SerializeField] bool jumpEnabled;
    [SerializeField] float jumpForce = 0.3f;
    [SerializeField] float jumpXAxisForce = 0f;
    [SerializeField] float RigidBodyCenterOffset = 0.05f;
    [SerializeField] BoxCollider2D groundCheck;
    [SerializeField] BoxCollider2D jumpCheck;

    [Header("Knockback")]
    [SerializeField] Vector2 knockbackForce = new Vector2(2, 3.5f);
    [SerializeField] float knockbackDuration = 0.8f;

    Path path;
    int currentWaypoint = 0;
    float lastDirection = 1f;
    float direction = 1f;
    Seeker seeker;
    Rigidbody2D rb;
    Health playerHealth;
    Health enemyHealth;
    Animator animator;

    bool isKnockedBack = false;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();

        if (target == null) target = FindObjectOfType<PlayerCombat>().gameObject;
        playerHealth = target.GetComponent<Health>();


        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (!isKnockedBack && enemyHealth.isAlive)//making sure the knockback works properly
        {
            if (TargetInDistance() && playerHealth.isAlive) PathFollow();
            else rb.velocity = new Vector3(0, rb.velocity.y, 0);//stop enemy if player not in range or dead
        }
        Animate();
    }

    #region SettingPath
    void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())//check if previous path is done generating
        {
            seeker.StartPath(rb.position - new Vector2(0, RigidBodyCenterOffset), target.transform.position - new Vector3(0, 0.2f, 0), OnPathComplete);
        }
    }

    private bool TargetInDistance()
    {
        if (target == null) return false;
        return (Vector2.Distance(transform.position, target.transform.position) < activateDistance);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    #endregion

    #region movement
    void PathFollow()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) return;
        
        //Calculate the direction
        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).x;

        // Determine if direction has changed significantly
        if (Mathf.Abs(direction) > 0.4f)
        {
            direction = Mathf.Sign(direction);
        }
        else
        {
            direction = lastDirection;
        }

        if (direction != lastDirection)//Rotate enemy
        {
            lastDirection = direction;
            if (direction < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        float predictedDistanceY;
        if (path.vectorPath.Count > currentWaypoint + pathPredictScale)
        {
            predictedDistanceY = path.vectorPath[currentWaypoint + pathPredictScale].y - (rb.position - new Vector2(0, RigidBodyCenterOffset)).y;//make sure the path is going upwards
        }
        else predictedDistanceY = path.vectorPath[currentWaypoint].y - (rb.position - new Vector2(0, RigidBodyCenterOffset)).y;

        Debug.Log(jumpEnabled + " jump enabled " + groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground")) + " groundCheck " + jumpCheck.IsTouchingLayers(LayerMask.GetMask("Ground")) + " jumpCheck");

        //check if enemy should jump now
        //1: check if jump enabled 2:check if enemy is on the ground 3: check if there is any obstacle next to enemy to jump on
        if (jumpEnabled && 
            (groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground"))
            && jumpCheck.IsTouchingLayers(LayerMask.GetMask("Ground")) 
            && predictedDistanceY > jumpNodeHeightRequirement))
        {
            Debug.Log("Jump conditions met");
            Debug.Log(Time.deltaTime + " Time.deltaTime");
            Debug.Log(predictedDistanceY + " predictedDistanceY");
            rb.velocity += new Vector2(0f, jumpForce);
        }

        if (groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground"))) rb.velocity = new Vector2((direction * speed * Time.deltaTime), rb.velocity.y);
        else rb.velocity = new Vector2((direction * jumpXAxisForce * speed * Time.deltaTime), rb.velocity.y);

        //update next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;
    }
    #endregion

    #region knockback
    //Taking care about knockback on the enemy
    public void ApplyKnockbackEnemy()
    {
        StartCoroutine(KnockbackCoroutine());
    }

    private IEnumerator KnockbackCoroutine() //waiting for duration so that positive x force doesnt get applied mid-air;
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;

        Vector2 force = new Vector2(-direction * knockbackForce.x, knockbackForce.y);//uses the negative direction so the enemy goes back
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
    #endregion

    #region animations

    void Animate()
    {
        if (rb.velocity.y > 0.1) { animator.SetFloat("velocity.y", 1); animator.SetBool("isJumping", true); }
        else if (rb.velocity.y < -0.1) { animator.SetFloat("velocity.y", -1); animator.SetBool("isJumping", true); }
        else { animator.SetFloat("velocity.y", 0); animator.SetBool("isJumping", false); }

        if (Mathf.Abs(rb.velocity.x) > 0.1) animator.SetBool("isRunning", true);
        else animator.SetBool("isRunning", false);
    }
    #endregion
}
