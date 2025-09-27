using UnityEngine;

public class TDGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float cellSize = 1f;

    [Header("Tile Settings")]
    public Sprite tileSprite;
    public Color colorEven = Color.white;
    public Color colorOdd = Color.gray;

    private GameObject[,] gridTiles;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // Clear existing grid
        if (gridTiles != null)
        {
            foreach (GameObject tile in gridTiles)
            {
                if (tile != null)
                    Destroy(tile);
            }
        }

        gridTiles = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject tile = new GameObject($"Tile_{x}_{y}");
                tile.transform.parent = transform;
                tile.transform.position = new Vector3(x * cellSize, y * cellSize, 0);

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = tileSprite;
                sr.sortingOrder = -1; 

                if ((x + y) % 2 == 0)
                    sr.color = colorEven;
                else
                    sr.color = colorOdd;

                gridTiles[x, y] = tile;
            }
        }
    }
}
