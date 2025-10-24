using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public float cellSize = 1f;
    public int width = 10;
    public int height = 10;
    public Vector2 origin = Vector2.zero;
    [SerializeField] private Camera mainCamera;
    public Vector3Int cell;

    public readonly Dictionary<Vector3Int, PlaceObject2D> occupied = new Dictionary<Vector3Int, PlaceObject2D>();

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f; 

        cell = WorldToCell(mouseWorld);

        //Debug.Log($"Cell Position: {cell}");

    }

    public Vector3Int WorldToCell(Vector3 world)
    {
        Vector2 local = (Vector2)world - origin;
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.y / cellSize);
        return new Vector3Int(x, y, 0);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return new Vector3(origin.x + (cell.x + 0.5f) * cellSize, origin.y + (cell.y + 0.5f) * cellSize, 0f);
    }

    public bool IsCellOccupied(Vector3Int cell)
    {
        return occupied.ContainsKey(cell);
    }

    public void SetOccupied(IEnumerable<Vector3Int> cell, PlaceObject2D obj)
    {
        foreach (var c in cell)
        {
            occupied[c] = obj;
        }
    }

    public void ClearOccupied(IEnumerable<Vector3Int> cell)
    {
        foreach (var c in cell)
        {
            occupied.Remove(c);
        }
    }

}
