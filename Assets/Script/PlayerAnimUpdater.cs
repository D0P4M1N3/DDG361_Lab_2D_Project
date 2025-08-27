using UnityEngine;

public class PlayerAnimUpdater : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    void Update()
    {
        float speedX = Mathf.Abs(playerController.rb.linearVelocity.x);

        anim.SetFloat("Speed", speedX);
        anim.SetBool("isGrounded", playerController.isGrounded);

        if (playerController.moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (playerController.moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

    }
}
