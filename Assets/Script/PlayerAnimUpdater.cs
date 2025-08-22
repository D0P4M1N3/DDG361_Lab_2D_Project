using UnityEngine;

public class PlayerAnimUpdater : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator anim;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float speedX = Mathf.Abs(playerController.rb.linearVelocity.x);

        anim.SetFloat("Speed", speedX);
        anim.SetBool("isGrounded", playerController.isGrounded);

    }
}
