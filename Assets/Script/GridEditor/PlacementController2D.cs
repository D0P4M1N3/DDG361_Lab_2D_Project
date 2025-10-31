using Builder2D;
using UnityEngine;
using System.Collections.Generic;

public class PlacementController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GridSystem gridSystem;

    [Header("Placement Settings")]
    [SerializeField] private float cellSize = 1f;

    [Header("Buildables")]
    [SerializeField] private BuildableDefinitionSO[] sampleDefinitions;

    private BuildableDefinitionSO buildableDefinition;
    private GameObject previewObject;
    private int rotationSteps = 0;
    private PlaceObject2D selectedObject; 

    void Start()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!gridSystem) gridSystem = FindObjectOfType<GridSystem>();
    }

    void Update()
    {
        HandleBuildableSelection();
        HandleRotation();
        UpdatePreviewPosition();
        HandlePlacement();
        Hover(); 
    }

    private void HandleBuildableSelection()
    {
        for (int i = 0; i < sampleDefinitions.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (previewObject) Destroy(previewObject);

                buildableDefinition = sampleDefinitions[i];
                previewObject = Instantiate(buildableDefinition.Prefab);
                SetPreviewMode(previewObject, true);
                rotationSteps = 0;
                selectedObject = null;
            }
        }
    }

    private void HandleRotation()
    {
        if (previewObject && Input.GetKeyDown(KeyCode.R))
            rotationSteps = (rotationSteps + 1) % 4;
    }

    private void UpdatePreviewPosition()
    {
        if (!previewObject || buildableDefinition == null) return;

        Vector3Int baseCell = gridSystem.WorldToCell(GetMouseWorldPosition());
        var footprint = buildableDefinition.GatherCells(baseCell, rotationSteps);

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;
        foreach (var c in footprint)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
            if (c.x > maxX) maxX = c.x;
            if (c.y > maxY) maxY = c.y;
        }

        float centerCellX = (minX + maxX) / 2f;
        float centerCellY = (minY + maxY) / 2f;

        Vector3 previewPos = new Vector3(
            gridSystem.origin.x + (centerCellX + 0.5f) * gridSystem.cellSize,
            gridSystem.origin.y + (centerCellY + 0.5f) * gridSystem.cellSize,
            0f
        );

        previewObject.transform.position = previewPos;
        previewObject.transform.rotation = Quaternion.Euler(0, 0, rotationSteps * VectorUtils.ROTATION_STEP_ANGLE);

        bool canPlace = true;
        foreach (var c in footprint)
        {
            if (!gridSystem.IsCellInBounds(c) || gridSystem.IsCellOccupied(c))
            {
                canPlace = false;
                break;
            }
        }

        SetPreviewColor(previewObject, canPlace);
    }

    private void HandlePlacement()
    {
        if (!previewObject || buildableDefinition == null) return;
        if (!Input.GetMouseButton(0)) return;

        Vector3Int baseCell = gridSystem.WorldToCell(GetMouseWorldPosition());
        var footprint = buildableDefinition.GatherCells(baseCell, rotationSteps);

        foreach (var c in footprint)
        {
            if (!gridSystem.IsCellInBounds(c) || gridSystem.IsCellOccupied(c))
                return;
        }

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;
        foreach (var c in footprint)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
            if (c.x > maxX) maxX = c.x;
            if (c.y > maxY) maxY = c.y;
        }

        float centerCellX = (minX + maxX) / 2f;
        float centerCellY = (minY + maxY) / 2f;

        Vector3 placePos = new Vector3(
            gridSystem.origin.x + (centerCellX + 0.5f) * gridSystem.cellSize,
            gridSystem.origin.y + (centerCellY + 0.5f) * gridSystem.cellSize,
            0f
        );

        GameObject placedObject = Instantiate(buildableDefinition.Prefab, placePos, Quaternion.Euler(0, 0, rotationSteps * VectorUtils.ROTATION_STEP_ANGLE));
        PlaceObject2D placedData = placedObject.AddComponent<PlaceObject2D>();
        placedData.buildableDefinition = buildableDefinition;
        placedData.RotationStep = rotationSteps;
        placedData.OriginCell = baseCell;

        gridSystem.SetOccupied(footprint, placedData);

        if (selectedObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
            selectedObject = null;
        }
    }

    private void Hover()
    {
        Vector3Int hoveredCell = gridSystem.WorldToCell(GetMouseWorldPosition());

        if (!gridSystem.occupied.TryGetValue(hoveredCell, out PlaceObject2D obj))
            return;

        if (Input.GetKey(KeyCode.D))
        {
            IEnumerable<Vector3Int> occupiedCells = obj.OccupiedCells(gridSystem);
            gridSystem.ClearOccupied(occupiedCells);
            Destroy(obj.gameObject);
            return;
        }

        if (Input.GetKey(KeyCode.R))
        {
            gridSystem.ClearOccupied(obj.OccupiedCells(gridSystem));

            selectedObject = obj;
            buildableDefinition = obj.buildableDefinition;
            rotationSteps = obj.RotationStep;

            if (previewObject) Destroy(previewObject);
            previewObject = Instantiate(buildableDefinition.Prefab);
            SetPreviewMode(previewObject, true);

            Destroy(obj.gameObject); 
        }

    }

    private void SetPreviewColor(GameObject obj, bool valid)
    {
        Color color = valid ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f, 0.5f);
        foreach (var r in obj.GetComponentsInChildren<SpriteRenderer>())
            r.color = color;
    }

    private void SetPreviewMode(GameObject obj, bool active)
    {
        foreach (var r in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = r.color;
            c.a = active ? 0.5f : 1f;
            r.color = c;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
