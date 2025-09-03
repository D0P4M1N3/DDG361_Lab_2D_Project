using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float moveInput; 
    public bool jumpInput;
    public bool dashInput;

    private void Update()
    {
        GetMovementInput();
        GetJumpInput();
        GetDashInput();
    }

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


    private void GetDashInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            dashInput = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            dashInput = false;
        }
    }


}
