using TMPro;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private int maxHeld = 20;
    [SerializeField] private int currentHeld = 0; // start full

    private TextMeshProUGUI resourceText;

    void Start()
    {
        resourceText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();

        currentHeld = maxHeld; // start full
    }

    void Update()
    {
        if (resourceText != null)
        {
            UpdateText();
        }
    }

    public bool IsEmpty()
    {
        return currentHeld <= 0;
    }

    public int Take(int amount)
    {
        int taken = Mathf.Min(amount, currentHeld);
        currentHeld -= taken;
        UpdateText();
        return taken;
    }

    private void UpdateText()
    {
        if (resourceText != null)
        {
            resourceText.text = $"{currentHeld}/{maxHeld}";
        }
    }
}
