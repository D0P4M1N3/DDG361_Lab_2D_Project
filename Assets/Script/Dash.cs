using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dash : Ability
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private HorizontalMovement horizontalMovement;

    private bool isDashing;
    private float dashTimer;
    private float originalGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        horizontalMovement = GetComponent<HorizontalMovement>();
        originalGravity = rb.gravityScale;
    }

    public override void Activate(PlayerInput input) //might work without from player input
    {
        if (!isDashing && input.DashPressed && CanUse())
        {
            StartDash();
        }

        if (isDashing)
        {
            UpdateDash();

        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        StartCooldown(dashCooldown);

        rb.gravityScale = 0f;
    }

    private void UpdateDash()
    {
        dashTimer -= Time.deltaTime;
        rb.linearVelocity = new Vector2(horizontalMovement.moveDirection * dashSpeed, 0f);

        if (dashTimer <= 0f)
        {
            EndDash();

        }
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public override bool IsActive() => isDashing;
}
