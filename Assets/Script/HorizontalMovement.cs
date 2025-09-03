using UnityEngine;

public class HorizontalMovement : MonoBehaviour
{
    public int piority = 0;

    public float moveDirection;
    private Rigidbody2D rb;
    private Ability ability;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public bool canMoveForward = true;

    [Header("Facing Direction")]
    public int facingDir { get; private set; } = 1; 

    private void Start()
    {
        ability = GetComponent<Ability>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDirection = ability.moveDir;

        if (moveDirection > 0)
        {
            facingDir = 1;
        }
        else if (moveDirection < 0)
        {
            facingDir = -1;
        }

        if (canMoveForward)
        {
            Move();
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection != 0)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }
}
