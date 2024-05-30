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
        if(rb.velocity.x > 0)Debug.Log(rb.velocity);
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
        if (direction < 0)
        {
            direction = -1;//Eliminates issues when enemy is above the player
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            direction = 1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }


        float predictedDistanceY;
        if (path.vectorPath.Count > currentWaypoint + 2)
        {
            predictedDistanceY = path.vectorPath[currentWaypoint + 2].y - (rb.position - new Vector2(0, RigidBodyCenterOffset)).y;
        }
        else predictedDistanceY = path.vectorPath[currentWaypoint].y - (rb.position - new Vector2(0, RigidBodyCenterOffset)).y;


        if (jumpEnabled && groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground")) && jumpCheck.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (predictedDistanceY > jumpNodeHeightRequirement)
            {
                Debug.Log(predictedDistanceY);
                rb.velocity += new Vector2(0f, jumpForce);
            }
        }

        rb.velocity = new Vector2(direction * speed * Time.deltaTime, rb.velocity.y);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;

        //if (rb.velocity.x > 0.2f) transform.rotation = Quaternion.Euler(0, 0, 0);//transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //else if (rb.velocity.x < -0.2f) transform.rotation = Quaternion.Euler(0, 180, 0);//transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
