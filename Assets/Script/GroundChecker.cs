using UnityEngine;

public class GroundChecker : MonoBehaviour
{

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0f, 0.5f)] public float stepHeight = 0.2f;
    [SerializeField] private Vector2 raycastOriginOffset = Vector2.zero;

    [Header("Slope Check")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 46f;
    [SerializeField] private Vector2 slopeRaycastOffset = Vector2.zero;

    [Header("Collider")]
    [SerializeField] private Vector2 boxColliderOffset = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 boxColliderSize = new Vector2(1f, 0.5f);

    public bool isGrounded;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;
    private HorizontalMovement horizontalMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        horizontalMovement = GetComponent<HorizontalMovement>();
    }

    private void Update()
    {
        GroundCheckAndSnap();

        if (boxCollider != null)
        {
            float newHeight = Mathf.Max(0.1f, 1f - stepHeight);
            boxColliderSize.y = newHeight;

            boxColliderOffset.y = (newHeight * 0.5f) + stepHeight;

            boxCollider.size = boxColliderSize;
            boxCollider.offset = boxColliderOffset;
        }
    }

    private void FixedUpdate()
    {
        GroundCheckAndSnap();
        ForwardSlopeCheck();

        if (!isGrounded)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            boxCollider.enabled = false;
            capsuleCollider.enabled = true;
        }
        else
        {
            if (horizontalMovement.moveDirection == 0)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            boxCollider.enabled = true;
            capsuleCollider.enabled = false;

        }
    }

    private void GroundCheckAndSnap() //Snap on different step terrain for ground movement charaters
    {
        float halfHeight = (boxCollider.size.y * stepHeight) * Mathf.Abs(transform.localScale.y);
        Vector2 colliderOffset = Vector2.Scale(boxCollider.offset, transform.localScale);

        Vector2 bottomCenter = (Vector2)transform.position + colliderOffset - new Vector2(0, halfHeight);
        Vector2 rayStart = bottomCenter + Vector2.up * stepHeight + raycastOriginOffset;

        int rayCount = 10;
        float raySpacing = boxCollider.size.x * Mathf.Abs(transform.localScale.x);

        bool hitSomething = false;
        float highestGround = float.NegativeInfinity;
        RaycastHit2D centerHit = new RaycastHit2D();

        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0 : (i / (float)(rayCount - 1) - 0.5f);
            Vector2 origin = rayStart + new Vector2(t * raySpacing, 0);

            Debug.DrawRay(origin, Vector2.down * (groundCheckDistance + stepHeight), (i == rayCount / 2) ? Color.red : Color.blue);

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance + stepHeight, groundLayer);

            if (hit.collider != null && rb.linearVelocity.y <= 0)
            {
                hitSomething = true;

                if (i == rayCount / 2 && hit.point.y > highestGround)
                {
                    highestGround = hit.point.y;
                    centerHit = hit;
                }
            }
        }

        if (hitSomething && centerHit.collider != null)
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

    private void ForwardSlopeCheck()
    {
        Vector2 origin = (Vector2)transform.position + slopeRaycastOffset;
        Vector2 direction = horizontalMovement.moveDirection > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, slopeCheckDistance, groundLayer);
        Debug.DrawRay(origin, direction * slopeCheckDistance, Color.green);

        if (hit.collider != null)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            horizontalMovement.canMoveForward = slopeAngle <= maxSlopeAngle;
        }
        else
        {
            horizontalMovement.canMoveForward = true;
        }
    }
}
