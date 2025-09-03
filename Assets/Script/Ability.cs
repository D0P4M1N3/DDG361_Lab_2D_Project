using UnityEngine;

[System.Serializable]
public class Ability : MonoBehaviour
{
    [Header("Player Input (auto-assigned)")]
    [SerializeField] private PlayerInput plrInput;

    [Header("Input States (updated every frame)")]
    public float moveDir { get; private set; }
    public bool jump { get; private set; }
    public bool dash { get; private set; }

    [Header("Ability Management")]
    public int AbilityPriority;

    private void Awake()
    {
        if (plrInput == null)
        {
            plrInput = GetComponent<PlayerInput>();

        }
    }

    private void Update()
    {
        moveDir = plrInput.moveInput;
        jump = plrInput.jumpInput;
        dash = plrInput.dashInput;
    }
}
