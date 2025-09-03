using UnityEngine;

[System.Serializable]

public class Ability : MonoBehaviour
{
    [SerializeField] private PlayerInput plrInput;
    public float moveDir;
    public bool jump;
    public bool sprint;

    public float AbilityPiority = 0f;   

    private void Awake()
    {
        plrInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        moveDir = plrInput.moveInput;
        jump = plrInput.jumpInput;
        sprint = plrInput.sprintInput;

    }
}
