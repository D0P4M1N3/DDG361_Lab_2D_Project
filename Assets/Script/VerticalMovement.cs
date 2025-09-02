using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Ability ability;
    private GroundChecker groundChecker;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpTime = 0.3f; // max time you can hold jump
    private float jumpTimeCounter;
    private bool isJumping;

    private float coyoteTimeCounter;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.1f;

    void Start()
    {
        groundChecker = GetComponent<GroundChecker>();
        ability = GetComponent<Ability>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (groundChecker.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            rb.gravityScale = 1;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;

            if (isJumping && ability.jump)
                rb.gravityScale = 1;
            else
                rb.gravityScale = 5;
        }

        HandleJump();
    }

    private void HandleJump()
    {
        if (ability.jump && coyoteTimeCounter > 0f)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }

        if (ability.jump && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (!ability.jump)
        {
            isJumping = false;
        }
    }
}
