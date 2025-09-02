using UnityEngine;

public class Dash : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDistance = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;


    private Rigidbody2D rb;
    private HorizontalMovement horizontalMovement;
    private Ability ability;

    private bool isDashing;  ///Get ability Priority from Ability.cs
    private float dashTime;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private float storedGravity = 1f;

    void Start()
    {
        rb= GetComponent<Rigidbody2D>();  
        horizontalMovement = GetComponent<HorizontalMovement>();
        ability = GetComponent<Ability>();
        
    }

    // Update is called once per frame
    void Update()
    {
        DashInput();
        HandleDash();
    }

    private void DashInput()
    {
        if (ability.sprint && !isDashing && dashCooldownTimer <= 0f)
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
        if (isDashing) ///Get ability Priority from Ability.cs
        {
            dashTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(horizontalMovement.moveDirection * dashSpeed, 0f);

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = storedGravity;
            }
        }
    }
}
