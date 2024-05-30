using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.WSA;
using UnityEditor;
using Cinemachine.Utility;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] Transform target;
    [SerializeField] float activateDistance = 50f;
    [SerializeField] float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    [SerializeField] float speed = 10f;
    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float jumpNodeHeightRequirement = 1f;
    [SerializeField] bool jumpEnabled;
    [SerializeField] float jumpForce = 0.3f;
    [SerializeField] float jumpCheckOffset;
    [SerializeField] float RigidBodyCenterOffset = 0.05f;
    [SerializeField] BoxCollider2D groundCheck;
    [SerializeField] BoxCollider2D jumpCheck;

    Path path;
    int currentWaypoint = 0;
    float lastDirection = 1f;
    Seeker seeker;
    Rigidbody2D rb;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance()) PathFollow();
    }

    void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position - new Vector2(0, RigidBodyCenterOffset), target.position - new Vector3(0, 0.2f, 0), OnPathComplete);
        }
    }

    void PathFollow()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) return;
        
        //Calculate the direction
        float direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).x;

        // Determine if direction has changed significantly
        if (Mathf.Abs(direction) > 0.1f)
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
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        float predictedDistanceY;
        if (path.vectorPath.Count > currentWaypoint + 2)
        {
            predictedDistanceY = path.vectorPath[currentWaypoint + 2].y - (rb.position - new Vector2(0, RigidBodyCenterOffset)).y;//make sure the path is going upwards
        }
        else predictedDistanceY = path.vectorPath[currentWaypoint].y - (rb.position - new Vector2(0, RigidBodyCenterOffset)).y;


        if (jumpEnabled && groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground")) && jumpCheck.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (predictedDistanceY > jumpNodeHeightRequirement)
            {
                rb.velocity += new Vector2(0f, jumpForce);
            }
        }

        rb.velocity = new Vector2(direction * speed * Time.deltaTime, rb.velocity.y);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;
    }

    private bool TargetInDistance()
    {
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
}
