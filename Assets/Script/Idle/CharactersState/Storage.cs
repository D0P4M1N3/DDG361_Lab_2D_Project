using TMPro;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private int maxHeld = 20;
    [SerializeField] private int currentHeld = 0;

    private TextMeshProUGUI storageText;

    void Start()
    {
        storageText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();

        currentHeld = 0; 
    }

    void Update()
    {
        if (storageText != null)
        {
            UpdateText();
        }
    }

    public bool IsFull()
    {
        return currentHeld >= maxHeld;
    }

    public void Store(int amount)
    {
        currentHeld += amount;
        if (currentHeld > maxHeld)
        {
            currentHeld = maxHeld;
        }
        UpdateText();
    }

    private void UpdateText()
    {
        if (storageText != null)
        {
            storageText.text = $"{currentHeld}/{maxHeld}";
        }
    }
}
