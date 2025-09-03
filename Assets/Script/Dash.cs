using UnityEngine;

public class Dash : MonoBehaviour
{
    [Header("Ability Settings")]
    public int priority = 0;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private HorizontalMovement horizontalMovement;
    private Ability ability;

    private bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        horizontalMovement = GetComponent<HorizontalMovement>();
        ability = GetComponent<Ability>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (!isDashing && ability.dash && cooldownTimer <= 0f)
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
        cooldownTimer = dashCooldown;

        rb.gravityScale = 0f;
        ability.AbilityPriority = priority;
    }

    private void UpdateDash()
    {
        dashTimer -= Time.deltaTime;

        if (ability.AbilityPriority > priority)
        {
            return;
        }

        rb.linearVelocity = new Vector2(horizontalMovement.facingDir * dashSpeed, 0f);
        ability.AbilityPriority = priority;


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

        ability.AbilityPriority = 0;
    }
}
