using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("References")]
    public PlayerInput playerInput;
    public HorizontalMovement horizontalMovement;
    public VerticalMovement verticalMovement;

    [SerializeField] private Ability[] abilities;

    private void Awake()
    {
        abilities = GetComponents<Ability>();
        playerInput = GetComponent<PlayerInput>();
        horizontalMovement = GetComponent<HorizontalMovement>();
        verticalMovement = GetComponent<VerticalMovement>();
    }

    private void Update()
    {
        playerInput.HandleInput();

        horizontalMovement.Move(playerInput.MoveDirection);
        verticalMovement.HandleJump(playerInput.JumpPressed, playerInput.JumpReleased);


        foreach (var ability in abilities)
        {
            ability.Activate(playerInput);

        }

    }
}
