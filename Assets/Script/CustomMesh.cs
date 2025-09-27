using UnityEngine;

public class CustomMesh : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private MeshFilter meshFilter;

    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;
    [SerializeField] private Vector2[] uv;


    void Start()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh; 

        DrawTriangle();

    }

    void Update()
    {

    }

    void DrawTriangle()
    {
        vertices = new Vector3[]
        {
            new Vector3(0,1,0),
            new Vector3(1,0,0),
            new Vector3(0,0,0)
        };

        triangles = new int[]
        {
            0,1,2
        };

        uv = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,1)
        };

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
    }

   

}
