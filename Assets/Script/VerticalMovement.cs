using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VerticalMovement : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpCutMultiplier = 0.5f; 

    private Rigidbody2D rb;
    private GroundChecker groundChecker;

    private float coyoteCounter;
    private float jumpBufferCounter;

    private Ability[] abilities;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponent<GroundChecker>();
        abilities = GetComponents<Ability>();
    }

    public void HandleJump(bool jumpPressed, bool jumpReleased)
    {
        foreach (var ability in abilities)
        {
            if (ability.IsActive() && ability.priority > 0)
                return;
        }

        if (groundChecker.isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        if (jumpReleased && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }
}
