using Builder2D;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject2D : MonoBehaviour
{
    public BuildableDefinitionSO buildableDefinition;
    public int RotationStep;
    public Vector3Int OriginCell;

    public IEnumerable<Vector3Int> OccupiedCells(GridSystem grid)
    {
        return buildableDefinition.GatherCells(OriginCell, RotationStep);
    }
}
