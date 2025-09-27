using UnityEngine;

public class TDCharacterController : MonoBehaviour
{
    public float gridSize = 1f; 
    private TDCharacterManager manager;

    private void Start()
    {
        manager = FindObjectOfType<TDCharacterManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Move(Vector3.up);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Move(Vector3.down);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector3.left);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector3.right);
    }

    private void Move(Vector3 dir)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = oldPos + dir * gridSize;
        transform.position = newPos;

        manager.RegisterMove(oldPos, newPos);
    }
}
