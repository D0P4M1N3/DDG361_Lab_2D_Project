using TMPro;
using UnityEngine;

public class Gatherer : MonoBehaviour
{
    [SerializeField] private int maxHeld = 8;
    public int held = 0;

    private TextMeshProUGUI heldText;
    private Movement movement;

    public float collectTime = 1f;

    [SerializeField] private Animator animator;


    public Vector3 TargetPosition { get; set; }
    public Resource TargetResource { get; set; }
    public Storage TargetStorage { get; set; }

    public bool isMoving = false;
    public bool isInteract = false;

    void Start()
    {
        heldText = GetComponentInChildren<TextMeshProUGUI>();
        movement = GetComponent<Movement>();
        
    }

    void Update()
    {
        if (heldText != null)
        {
            heldText.text = $"{held}/{maxHeld}";
        }

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("Interact", isInteract);

    }

    public void AddResource(int amount)
    {
        held = Mathf.Min(held + amount, maxHeld);
    }

    public void RemoveResource(int amount)
    {
        held = Mathf.Max(held - amount, 0);
    }

    public bool IsFull()
    {
        return held >= maxHeld;
    }

}
