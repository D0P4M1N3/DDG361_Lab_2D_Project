using UnityEngine;

internal class ChangeColor : MonoBehaviour, IInteractable
{
    private MeshRenderer meshRenderer;
    private CircleCollider2D circleCollider;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Interact()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            meshRenderer.material.color = randomColor;
            circleCollider.isTrigger = true;
        }
    }

}
