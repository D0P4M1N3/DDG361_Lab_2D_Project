using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class StepHandler : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float jumpHoldMultiplier = 0.3f;
    public float maxJumpHoldTime = 0.25f;

    [Header("Ground Detection")]
    public int rayCount = 5;
    public float rayLength = 0.2f;          // ray extension below collider
    public float maxSlopeAngle = 45f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isJumping;
    private float jumpTime;
    private bool isGrounded;
    private int lastMoveDir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleGroundSnap();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > 0.01f)
            lastMoveDir = h > 0 ? 1 : -1;

        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            jumpTime = maxJumpHoldTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetButton("Jump") && isJumping && jumpTime > 0f)
        {
            rb.linearVelocity += new Vector2(0f, jumpForce * jumpHoldMultiplier * Time.deltaTime);
            jumpTime -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump")) isJumping = false;
    }

    void HandleGroundSnap()
    {
        Vector2 bottomCenter = (Vector2)transform.position + col.offset - new Vector2(0, col.size.y * 0.5f);
        float halfWidth = col.size.x * 0.5f * Mathf.Abs(transform.lossyScale.x);

        int middleIndex = rayCount / 2;
        float groundY = float.NegativeInfinity;
        bool tooSteep = false;

        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0.5f : i / (float)(rayCount - 1);
            float offX = Mathf.Lerp(-halfWidth, halfWidth, t);
            Vector2 origin = bottomCenter + Vector2.right * offX;

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
            if (hit.collider != null)
            {
                if (i == middleIndex) // middle ray controls snap
                    groundY = hit.point.y;

                float slope = Vector2.Angle(hit.normal, Vector2.up);
                if (slope > maxSlopeAngle)
                    tooSteep = true;
            }
        }

        // snap only when falling or idle, not rising
        if (!isJumping && rb.linearVelocity.y <= 0 && groundY > float.NegativeInfinity)
        {
            isGrounded = true;

            // character�s bottom ? collider half height
            float charBottomToCenter = col.size.y * 0.5f;
            float targetY = groundY + charBottomToCenter;

            // smooth snap
            float newY = Mathf.MoveTowards(transform.position.y, targetY, 0.2f);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
        else
        {
            isGrounded = false;
        }

        // block too steep slopes
        if (tooSteep)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (col == null) return;

        Vector2 bottomCenter = (Vector2)transform.position + col.offset - new Vector2(0, col.size.y * 0.5f);
        float halfWidth = col.size.x * 0.5f * Mathf.Abs(transform.lossyScale.x);

        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0.5f : i / (float)(rayCount - 1);
            float offX = Mathf.Lerp(-halfWidth, halfWidth, t);
            Vector2 origin = bottomCenter + Vector2.right * offX;
            Gizmos.color = (i == rayCount / 2) ? Color.red : Color.yellow;
            Gizmos.DrawLine(origin, origin + Vector2.down * rayLength);
        }
    }
}
