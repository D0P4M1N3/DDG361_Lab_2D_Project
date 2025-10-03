using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Vector3 move = direction.normalized * speed * Time.deltaTime;
            transform.position += move;

            if (spriteRenderer != null)
            {
                if (move.x > 0f) spriteRenderer.flipX = false;
                else if (move.x < -0f) spriteRenderer.flipX = true;
            }
        }

    }
}
