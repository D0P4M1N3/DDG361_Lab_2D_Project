using UnityEngine;

public class Bush : MonoBehaviour
{
    [Header("Colors")]
    public Color highlightColor = Color.green; // color when player enters
    private Color defaultColor = Color.white;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultColor;
    }

    // Called when another collider enters this trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("Bush");
            spriteRenderer.color = highlightColor;
        }
    }

    // Called when another collider exits this trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("Default");

            spriteRenderer.color = defaultColor;
        }
    }
}
