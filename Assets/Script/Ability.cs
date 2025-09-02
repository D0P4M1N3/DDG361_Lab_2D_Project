using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] private PlayerInput plrInput;
    public float moveDir;
    public bool jump;
    public bool sprint;


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
