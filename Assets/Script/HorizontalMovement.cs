using UnityEditor.Playables;
using UnityEngine;

public class HorizontalMovement : MonoBehaviour
{
    public float moveDirection;
    private Rigidbody2D rb;

    private Ability ability;

    public float moveSpeed = 5f;
    public bool canMoveForward;

    private void Start()
    {
        ability = GetComponent<Ability>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDirection = ability.moveDir;

        if(canMoveForward)
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
        else if(moveDirection == 0)
        {
            rb.bodyType= RigidbodyType2D.Kinematic;
        }

    }

    public void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }


}
