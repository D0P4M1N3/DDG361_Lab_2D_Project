using UnityEngine;

public class PlayerAnimUpdater : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private HorizontalMovement horizontalMovement;
    private GroundChecker groundChecker;
    private Rigidbody2D rb;

    void Start()
    {
        horizontalMovement = GetComponentInParent<HorizontalMovement>();    
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        groundChecker = GetComponentInParent<GroundChecker>();
    }

    void Update()
    {
        float speedX = Mathf.Abs(rb.linearVelocity.x);

        anim.SetFloat("Speed", speedX);
        anim.SetBool("isGrounded", groundChecker.isGrounded);

        if (horizontalMovement.moveDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalMovement.moveDirection < 0)
        {
            spriteRenderer.flipX = true;
        }

    }
}
