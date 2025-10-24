using NUnit.Framework;
using System.Collections.Generic; 
using UnityEngine;


namespace Builder2D
{
    [CreateAssetMenu(fileName = "BuildableDefinition", menuName = "Scriptable Objects/BuildableDefinition")]

    public class BuildableDefinitionSO : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;

        [SerializeField] private Vector2Int baseFootprint = new Vector2Int(1,1);
        public Vector2Int BaseFootprint => baseFootprint;

        public List<Vector3Int> GatherCells(Vector3Int originCells, int rotationSteps)
        {
            var list = new List<Vector3Int>();
            Vector2Int fp = BaseFootprint;
            for(int y =0; y < fp.y; y++)
                for(int x = 0; x < fp.x; x++)
                    list.Add(originCells + VectorUtils.RotateOffset(new Vector3Int(x,y,0), rotationSteps));
            return list;
        }

    }

}
