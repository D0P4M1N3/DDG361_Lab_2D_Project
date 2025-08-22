using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f; 
    [SerializeField] private LayerMask layer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, layer);
        foreach (Collider2D hit in hits)
        {
            Debug.Log("Interacted with: " + hit.name);

            var interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
