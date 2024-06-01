using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int jumpAmount = 1;
    [SerializeField] float fallingScale = 1;

    [Header("Knockback")]
    [SerializeField] Vector2 knockbackForce = new Vector2(1, 2);
    [SerializeField] float knockbackDuration = 0.5f;

    private Vector2 input;
    private int currentJumps = 0;
    bool isKnockedBack = false;

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
        if (!isKnockedBack)
        {
            moveHorizontally();
            moveVertically();
        }   
    }

    #region moving

    void OnMove(InputValue value) //get the input vector
    {
        input = value.Get<Vector2>();
    }

    void moveHorizontally()//physically move and animate
    {
        Debug.Log(Time.deltaTime);
        rb.velocity = new Vector2(input.x * moveSpeed, rb.velocity.y);

        if (Mathf.Abs(input.x) > Mathf.Epsilon)
        {
            animator.SetBool("isRunning", true);
            if (input.x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);//flipping the sprite based on the direction
            else transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
    #region knockback
    public void ApplyKnockbackPlayer(float direction)//Applies knockback for the player uses the direction(-1 or 1) so that enemy can pass in its local scale so it goes knock away from the player no matter which way the player is facing
    {
        StartCoroutine(KnockbackCoroutine(new Vector2(knockbackForce.x * direction, knockbackForce.y), knockbackDuration));//use Coroutine so that player cant fully counter the knockback
    }

    private IEnumerator KnockbackCoroutine(Vector2 force, float duration) //waiting for duration so that positive x force doesnt get applied mid-air;
    {
        isKnockedBack = true;
        Debug.Log(Mathf.Sign(rb.velocity.x) + " sign");
        Debug.Log("gerbi");
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
    }
    #endregion
    #region slow
    public void SlowPlayer(float duration, float ratio)
    {
        StartCoroutine(Slow(duration, ratio));
    }

    private IEnumerator Slow(float duration, float ratio)
    {
        moveSpeed *= ratio;

        yield return new WaitForSeconds(duration);

        moveSpeed /= ratio;
    }
    #endregion
}
