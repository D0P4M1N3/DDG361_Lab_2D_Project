using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TDSpriteUpdater : MonoBehaviour
{
    private SpriteRenderer sr;
    private TDCharacterController controller;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<TDCharacterController>();
    }

    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            moveDir = Vector3.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            moveDir = Vector3.right;

        if (moveDir == Vector3.left)
            sr.flipX = true;
        else if (moveDir == Vector3.right)
            sr.flipX = false;
    }
}
