using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;

    private Vector2 input;
    
    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        moveHorizontally();
    }

    void OnMove(InputValue value) //get the input vector
    {
        input = value.Get<Vector2>();
    }

    void moveHorizontally()//physically move and animate
    {
        rb.velocity = new Vector2(input.x * moveSpeed, rb.velocity.y);

        if (Mathf.Abs(input.x) > Mathf.Epsilon)
        {
            animator.SetBool("isRunning", true);
            if (input.x < 0) transform.rotation = Quaternion.Euler(0,180,0); //flipping the sprite based on the direction
            else transform.rotation = Quaternion.Euler(0,0,0);
        }
        else animator.SetBool("isRunning", false);
    }
}
