using Builder2D;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (gridSystem == null)
            gridSystem = FindObjectOfType<GridSystem>();
    }

    void Update()
    {
        HandleBuildableSelection();
        HandleRotation();
        UpdatePreviewPosition();
        HandlePlacement();
        HandleSelectionActions();
    }

    private void HandleBuildableSelection()
    {
        for (int i = 0; i < sampleDefinitions.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (previewObject != null)
                    Destroy(previewObject);

                buildableDefinition = sampleDefinitions[i];
                previewObject = Instantiate(buildableDefinition.Prefab);
                SetPreviewMode(previewObject, true);
            }
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
            rotationSteps = (rotationSteps + 1) % 4;
    }
    private void UpdatePreviewPosition()
    {
        if (previewObject == null) return;

        Vector3Int cell = gridSystem.WorldToCell(GetMouseWorldPosition());
        Vector3 baseOffset = new Vector3((buildableDefinition.BaseFootprint.x - 1) * cellSize / 2f,
                                         (buildableDefinition.BaseFootprint.y - 1) * cellSize / 2f, 0f);
        Vector3 rotatedOffset = VectorUtils.RotateOffset(baseOffset, rotationSteps);
        Vector3 previewPos = gridSystem.CellToWorld(cell) + rotatedOffset;

        previewObject.transform.position = previewPos;
        previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationSteps * VectorUtils.ROTATION_STEP_ANGLE);

        // Update preview color based on placement validity
        var footprint = buildableDefinition.GatherCells(cell, rotationSteps);
        bool canPlace = true;
        foreach (var c in footprint)
            if (gridSystem.IsCellOccupied(c))
                canPlace = false;

        SetPreviewColor(previewObject, canPlace);
    }

    private void SetPreviewColor(GameObject obj, bool valid)
    {
        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = valid ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
    }

    private void SetPreviewMode(GameObject obj, bool active)
    {
        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = renderer.color;
            c.a = active ? 0.5f : 1f;
            renderer.color = c;
        }
    }

    private void HandlePlacement()
    {
        if (previewObject == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int originCell = gridSystem.WorldToCell(previewObject.transform.position);
            var footprint = buildableDefinition.GatherCells(originCell, rotationSteps);

            foreach (var c in footprint)
            {
                if (gridSystem.IsCellOccupied(c))
                {
                    Debug.LogWarning("Cannot place — cell occupied!");
                    return;
                }
            }

            GameObject placedObject = Instantiate(buildableDefinition.Prefab, previewObject.transform.position, previewObject.transform.rotation);
            PlaceObject2D placeData = placedObject.AddComponent<PlaceObject2D>();
            placeData.buildableDefinition = buildableDefinition;
            placeData.RotationStep = rotationSteps;
            placeData.OriginCell = originCell;

            gridSystem.SetOccupied(footprint, placeData);
        }
    }

    private void HandleSelectionActions()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        Vector3Int clickedCell = gridSystem.WorldToCell(GetMouseWorldPosition());
        PlaceObject2D obj = gridSystem.occupied.ContainsKey(clickedCell) ? gridSystem.occupied[clickedCell] : null;

        if (obj == null) return;

        if (Input.GetKey(KeyCode.M))
        {
            if (previewObject != null) Destroy(previewObject);

            buildableDefinition = obj.buildableDefinition;
            rotationSteps = obj.RotationStep;

            previewObject = Instantiate(buildableDefinition.Prefab);
            SetPreviewMode(previewObject, true);

            gridSystem.ClearOccupied(obj.OccupiedCells(null));
            Destroy(obj.gameObject);
        }

        if (Input.GetKey(KeyCode.Delete))
        {
            gridSystem.ClearOccupied(obj.OccupiedCells(null));
            Destroy(obj.gameObject);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
