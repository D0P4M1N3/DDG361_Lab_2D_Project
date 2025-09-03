using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float MoveDirection { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }  
    public bool DashPressed { get; private set; }  

    public void HandleInput()
    {
        MoveDirection = Input.GetAxisRaw("Horizontal");
        JumpPressed = Input.GetButtonDown("Jump");
        JumpReleased = Input.GetButtonUp("Jump");
        DashPressed = Input.GetKeyDown(KeyCode.LeftShift); 
    }
}
