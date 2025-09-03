using UnityEngine;

public class Dash : Ability
{
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isDashing;
    private float dashTimer;
    private HorizontalMovement horizontalMovement;
    private float gravityScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        horizontalMovement = GetComponent<HorizontalMovement>();
        gravityScale = rb.gravityScale;
    }

    public override void Activate(PlayerInput input)
    {
        if (isDashing)
        {


            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                isDashing = false;
                rb.gravityScale = gravityScale;
                //rb.bodyType = RigidbodyType2D.Dynamic;
            }
            return;
        }

        if (input.DashPressed && CanUse())
        {
            rb.linearVelocity = new Vector2(horizontalMovement.Facing * dashSpeed, 0);

            isDashing = true;
            dashTimer = dashDuration;
            StartCooldown();
        }

    }

    public override bool IsActive() => isDashing;
}
