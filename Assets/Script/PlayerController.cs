using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 stepCheckerOffset = Vector2.zero;

    public Rigidbody2D rb;
    public bool isGrounded;

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
        HandleInput();
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        

        if (isGrounded)
        {
            capsuleCollider.enabled = false;
            boxCollider.enabled = true;
        }
        else
        {
            capsuleCollider.enabled = true;
            boxCollider.enabled = false;
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

    private void HandleInput()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip sprite depending on direction
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}
