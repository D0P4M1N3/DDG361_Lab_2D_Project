using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TDColorChanger : MonoBehaviour
{
    private SpriteRenderer sr;
    private TDCharacterManager manager;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<TDCharacterManager>();
    }

    public void ChangeToRandomColor()
    {
        Color oldColor = sr.color;
        Color newColor = new Color(Random.value, Random.value, Random.value);
        sr.color = newColor;

        manager.RegisterColorChange(oldColor, newColor);
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }
}
