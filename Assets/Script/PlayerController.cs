using UnityEngine;
using UnityEngine.InputSystem.Android;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0f, 0.5f)] public float stepHeight = 0.2f;
    [SerializeField] private Vector2 raycastOriginOffset = Vector2.zero;

    [Header("Slope Check")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 46f;
    [SerializeField] private Vector2 slopeRaycastOffset = Vector2.zero;

    private bool canMoveForward = true;

    [Header("Collider")]
    [SerializeField] private Vector2 boxColliderOffset = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 boxColliderSize = new Vector2(1f, 0.5f);

    [Header("Sprite")]
    [SerializeField] private GameObject characterSprite;
    [SerializeField] private Vector3 spriteOffset = Vector3.zero;

    [Header("Jump Buffer & Coyote Time")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.05f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    public Rigidbody2D rb;
    public bool isGrounded;
    public float moveInput;

    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        rb = GetComponent<Rigidbody2D>();

        SetupCharacter();
    }

    void Update()
    {
        UpdateSprite();
        SetupCharacter();
        GroundCheckAndSnap();
        MovementInput();
        JumpInput();
    }

    private void FixedUpdate()
    {
        GroundCheckAndSnap();
        if (!isGrounded)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            boxCollider.enabled = false;
            capsuleCollider.enabled = true;
        }
        else
        {
            if (moveInput != 0)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            boxCollider.enabled = true;
            capsuleCollider.enabled = false;
        }
    }

    private void GroundCheckAndSnap()
    {
        float halfHeight = (boxCollider.size.y * stepHeight) * Mathf.Abs(transform.localScale.y);
        Vector2 colliderOffset = Vector2.Scale(boxCollider.offset, transform.localScale);

        Vector2 bottomCenter = (Vector2)transform.position + colliderOffset - new Vector2(0, halfHeight);

        Vector2 rayOrigin = bottomCenter + Vector2.up * stepHeight + raycastOriginOffset;

        Debug.DrawRay(rayOrigin, Vector2.down * (groundCheckDistance + stepHeight), Color.blue);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance + stepHeight, groundLayer);

        if (hit.collider != null && rb.linearVelocity.y <= 0)
        {
            isGrounded = true;

            float targetY = hit.point.y + halfHeight;
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
            float newHeight = Mathf.Max(0.1f, 1f - stepHeight);
            boxColliderSize.y = newHeight;

            boxColliderOffset.y = (newHeight * 0.5f) + stepHeight;

            boxCollider.size = boxColliderSize;
            boxCollider.offset = boxColliderOffset;
        }
    }

    private void MovementInput()
    {
        moveInput = Input.GetAxis("Horizontal");
        ForwardSlopeCheck();

        if (canMoveForward)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void JumpInput()
    {
        // --- Handle coyote time ---
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // --- Handle jump buffer ---
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // --- Perform jump if conditions met ---
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Distinguish between jump sources
            if (!isGrounded && coyoteTimeCounter > 0)
            {
                Debug.Log("JUMP by COYOTE 🟡");
            }
            else if (isGrounded && jumpBufferCounter > 0)
            {
                Debug.Log("JUMP by BUFFER 🔵");
            }
            else
            {
                Debug.Log("NORMAL JUMP ✅");
            }

            jumpBufferCounter = 0f; // reset buffer after jump
        }

        // --- Variable jump height ---
        if (Input.GetButton("Jump") && rb.linearVelocity.y > 0)
        {
            rb.gravityScale = 2.5f;
        }
        else if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * stepHeight);
            rb.gravityScale = 5f;
        }
        else
        {
            rb.gravityScale = 5f;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        float halfHeight = (boxColliderSize.y * 0.5f) * Mathf.Abs(transform.lossyScale.y);
        Vector2 colliderOffset = Vector2.Scale(boxColliderOffset, transform.lossyScale);
        Vector2 bottomCenter = (Vector2)transform.position + colliderOffset - new Vector2(0, halfHeight);

        Vector2 rayOrigin = bottomCenter + Vector2.up * stepHeight + raycastOriginOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * (groundCheckDistance + stepHeight));
    }

    private void UpdateSprite()
    {
        characterSprite.transform.position = transform.position + spriteOffset;
    }

    private void ForwardSlopeCheck()
    {
        Vector2 origin = (Vector2)transform.position + slopeRaycastOffset;
        Vector2 direction = moveInput > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, slopeCheckDistance, groundLayer);
        Debug.DrawRay(origin, direction * slopeCheckDistance, Color.green);

        if (hit.collider != null)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            canMoveForward = slopeAngle <= maxSlopeAngle;
        }
        else
        {
            canMoveForward = true;
        }
    }
}
