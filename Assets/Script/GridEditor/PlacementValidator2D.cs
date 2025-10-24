using System.Collections.Generic;
using UnityEngine;



public class PlacementValidator2D : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    public bool CanPlaceAtCells(IEnumerable<Vector3Int> cells)
    {
        foreach (var cell in cells)
        {
            if (gridSystem.IsCellOccupied(cell))
            {
                return false;
            }
        }
        return true;
    }


}
