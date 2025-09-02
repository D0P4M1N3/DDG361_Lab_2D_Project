using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float moveInput; 
    public bool jumpInput;
    public bool sprintInput;

    private void GetMovementInput()
    {
        moveInput = Input.GetAxis("Horizontal");
    }

    private void GetJumpInput()
    {
        if (Input.GetButton("Jump"))
        {
            jumpInput = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpInput = false;
        }
    }


    private void Update()
    {
        GetMovementInput();
        GetJumpInput();
    }

    private void GetDashInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprintInput = true;
        }
        else
        {
            sprintInput= false;
        }
    }


}
