using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int jumpAmount = 1;
    [SerializeField] float fallingScale = 1;

    private Vector2 input;
    private int currentJumps = 0;

    BoxCollider2D groundCheck;
    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundCheck = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        moveHorizontally();
        moveVertically();
    }

    #region moving

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

    #endregion
    #region jumping

    void OnJump(InputValue value) //check if player should be able to jump
    {
        if (currentJumps < jumpAmount)
        {
            currentJumps++;
            Jump();
        }
        else if (groundCheck.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            currentJumps = 1;
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity += new Vector2(0f, jumpForce + (-rb.velocity.y)); //add the negative y velocity for the double jump to work properly
    }

    void moveVertically() //take care of the animations and add the falling scale
    {
        if (rb.velocity.y > 0.1) { animator.SetFloat("velocity.y", 1); animator.SetBool("isJumping", true); }
        else if (rb.velocity.y < -0.1) { animator.SetFloat("velocity.y", -1); animator.SetBool("isJumping", true); }
        else { animator.SetFloat("velocity.y", 0); animator.SetBool("isJumping", false); }

        if (rb.velocity.y < 0)
        {
            rb.velocity -= new Vector2(0f, fallingScale);
        }
    }

    #endregion
}
