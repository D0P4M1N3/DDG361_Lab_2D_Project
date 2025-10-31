using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;
    public int width = 10;
    public int height = 10;
    public Vector2 origin = Vector2.zero;

    [Header("References")]
    [SerializeField] private Camera mainCamera;

    public readonly Dictionary<Vector3Int, PlaceObject2D> occupied = new();
    public Vector3Int cell { get; private set; }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        cell = WorldToCell(mouseWorld);
    }

    // --- Coordinate Conversion ---
    public Vector3Int WorldToCell(Vector3 world)
    {
        Vector2 local = (Vector2)world - origin;
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.y / cellSize);
        return new Vector3Int(x, y, 0);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return new Vector3(
            origin.x + (cell.x + 0.5f) * cellSize,
            origin.y + (cell.y + 0.5f) * cellSize,
            0f
        );
    }

    // --- Occupancy & Validation ---
    public bool IsCellOccupied(Vector3Int cell)
    {
        return occupied.ContainsKey(cell);
    }

    public bool IsCellInBounds(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < width &&
               cell.y >= 0 && cell.y < height;
    }

    public void SetOccupied(IEnumerable<Vector3Int> cells, PlaceObject2D obj)
    {
        foreach (var c in cells)
        {
            if (!IsCellInBounds(c))
            {
                Debug.LogWarning($"❌ Cannot occupy cell {c} — outside grid bounds!");
                continue;
            }

            if (!occupied.ContainsKey(c))
                occupied[c] = obj;
            else
                Debug.LogWarning($"⚠️ Cell {c} already occupied when placing {obj.name}");
        }
    }

    public void ClearOccupied(IEnumerable<Vector3Int> cells)
    {
        foreach (var c in cells)
        {
            if (occupied.ContainsKey(c))
                occupied.Remove(c);
        }
    }

    public PlaceObject2D GetObjectAtCell(Vector3Int cell)
    {
        occupied.TryGetValue(cell, out var obj);
        return obj;
    }


    // --- Gizmo Debug View ---
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 center = CellToWorld(new Vector3Int(x, y, 0));
                Gizmos.DrawWireCube(center, Vector3.one * cellSize);
            }
        }

        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1f, 0.4f, 0f, 0.5f);
            foreach (var kvp in occupied)
            {
                Vector3 center = CellToWorld(kvp.Key);
                Gizmos.DrawCube(center, Vector3.one * (cellSize * 0.8f));
            }
        }
    }
}
