using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HorizontalMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private GroundChecker groundChecker;

    public float moveDirection;
    public bool canMoveForward;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponent<GroundChecker>();
    }

    public void Move(float direction)
    {
        if (canMoveForward)
        {
            moveDirection = direction;

            if (direction != 0)
            {
                moveDirection = Mathf.Sign(direction);
            }

            Vector2 velocity = rb.linearVelocity;
            velocity.x = direction * moveSpeed;
            rb.linearVelocity = velocity;
            rb.gravityScale = 5f;
        }
    }
}
