using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpTimer = 0.3f;
    private float jumpTimerCount = 0;


    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 stepCheckerOffset = Vector2.zero;
    [SerializeField] private float rayCastLength = 0f;

    [Header("Jump Buffering")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteCounter;

    public Rigidbody2D rb;
    public bool isGrounded;
    public float moveInput;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        SetupCharacter();
    }

    void Update()
    {
        MovementInput();
        JumpInput();

    }

    private void FixedUpdate()
    {
        GroundCheckAndSnap();

        if (!isGrounded)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void GroundCheckAndSnap()
    {

        Vector2 bottomCenter = (Vector2)transform.position + boxCollider.offset - new Vector2(0, boxCollider.size.y * 0.5f);
        float halfWidth = boxCollider.size.x * 0.5f * Mathf.Abs(transform.lossyScale.x);

        float groundY = float.NegativeInfinity;

        for (int i = 0; i < 2; i++)
        {
            float t = i / 2f;
            float offX = Mathf.Lerp(-halfWidth, halfWidth, t);
            Vector2 origin = bottomCenter + Vector2.right * offX;

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
            if (hit.collider != null && i == 1)
                groundY = hit.point.y;
        }

        if (groundY > float.NegativeInfinity && rb.linearVelocity.y <= 0)
        {
            isGrounded = true;

            float charBottomToCenter = boxCollider.size.y * 0.5f;
            float targetY = groundY + charBottomToCenter;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, 0.2f);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
        else
        {
            isGrounded = false;
        }
    }


    private void SetupCharacter()
    {
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(0.95f, 0.65f);
            boxCollider.offset = new Vector2(0, 0.3f);
        }

        if (capsuleCollider != null)
        {
            capsuleCollider.size = new Vector2(0.95f, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    private void MovementInput()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
        {
            rb.gravityScale = 5;
        }

        else
        {
            rb.gravityScale = 1;
        }

    }

    private void JumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetButton("Jump") && rb.linearVelocity.y > 0)
        {
            rb.gravityScale = 2.5f;
        }


        else if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            rb.gravityScale = 5f;
        }

        else
        {
            rb.gravityScale = 5f;
        }
    }


}
