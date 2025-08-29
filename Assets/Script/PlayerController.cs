using Unity.VisualScripting;
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

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDistance = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    private float dashTime;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private float storedGravity = 1f;

    public Rigidbody2D rb;
    public bool isGrounded;
    public float moveInput;

    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;

    private int facingDirection = 1;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        rb = GetComponent<Rigidbody2D>();

        SetupCharacter();

        dashTime = dashDistance / dashSpeed;
    }

    void Update()
    {
        UpdateSprite();
        SetupCharacter();
        GroundCheckAndSnap();

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing)
        {
            MovementInput();
            JumpInput();
        }

        DashInput();
        HandleDash();



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
        Vector2 rayStart = bottomCenter + Vector2.up * stepHeight + raycastOriginOffset;

        // Settings for multiple rays
        int rayCount = 10;
        float raySpacing = boxCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);

        bool hitSomething = false;
        float highestGround = float.NegativeInfinity;
        RaycastHit2D bestHit = new RaycastHit2D();

        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0 : (i / (float)(rayCount - 1) - 0.5f);
            Vector2 origin = rayStart + new Vector2(t * raySpacing, 0);

            Debug.DrawRay(origin, Vector2.down * (groundCheckDistance + stepHeight), Color.blue);

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance + stepHeight, groundLayer);

            if (hit.collider != null && rb.linearVelocity.y <= 0)
            {
                hitSomething = true;

                if (hit.point.y > highestGround)
                {
                    highestGround = hit.point.y;
                    bestHit = hit;
                }
            }
        }

        if (hitSomething)
        {
            isGrounded = true;

            float targetY = highestGround + halfHeight;
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

        
        if (moveInput > 0.1f)
        {
            facingDirection = 1;
            
        }
        else if (moveInput < -0.1f)
        {
            facingDirection = -1;
            
        }
    }

    private void JumpInput()
    {
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (!isGrounded && coyoteTimeCounter > 0)
            {
                Debug.Log("JUMP by COYOTE");
            }
            else if (isGrounded && jumpBufferCounter > 0)
            {
                Debug.Log("JUMP by BUFFER");
            }
            else
            {
                Debug.Log("NORMAL JUMP");
            }

            jumpBufferCounter = 0f;
        }

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

    private void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashTime;
            dashCooldownTimer = dashCooldown;

            storedGravity = rb.gravityScale;
            rb.gravityScale = 0f;
        }
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = storedGravity;
            }
        }
    }


}
