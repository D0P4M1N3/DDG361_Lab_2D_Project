using UnityEngine;

public class Dash : MonoBehaviour
{
    public float piority = 0f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private HorizontalMovement horizontalMovement;
    private Ability ability;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private float storedGravity;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        horizontalMovement = GetComponent<HorizontalMovement>();
        ability = GetComponent<Ability>();
    }

    void Update()
    {
        dashCooldownTimer -= Time.deltaTime;

        DashInput();
        HandleDash();
    }

    private void DashInput()
    {
        if (ability.sprint && !isDashing && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashDuration;
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
            if (ability.AbilityPiority <= piority)
            {
                ability.AbilityPiority = piority;

                rb.linearVelocity = new Vector2(horizontalMovement.facingDir * dashSpeed, 0f);

                if (dashTimer <= 0f)
                {
                    isDashing = false;
                    rb.gravityScale = storedGravity;

                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                    ability.AbilityPiority = 0f;
                }
            }
        }
    }

}

