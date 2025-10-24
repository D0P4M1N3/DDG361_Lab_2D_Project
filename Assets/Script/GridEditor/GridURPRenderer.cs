using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(GridSystem))]
public class GridURPRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    private GridSystem grid;

    [Header("Appearance")]
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color majorLineColor = new Color(1f, 1f, 1f, 0.6f);
    [Tooltip("Draw a stronger line every N cells (0/1 = disabled).")]
    [SerializeField] private int majorEvery = 5;
    [SerializeField] private float zOffset = 0.01f;

    [Header("Toggle")]
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private bool drawBounds = true;

    private static Material lineMaterial;

    private void OnEnable()
    {
        grid = GetComponent<GridSystem>();
        if (targetCamera == null)
            targetCamera = Camera.main;

        EnsureMaterial();
    }

    private void EnsureMaterial()
    {
        if (lineMaterial != null) return;

        // Use our custom URP-safe shader
        Shader shader = Shader.Find("Hidden/GridLineURP");
        if (shader == null)
        {
            Debug.LogError("Shader 'Hidden/GridLineURP' not found! Please add GridLineURP.shader to your project.");
            return;
        }

        lineMaterial = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        lineMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    private void OnRenderObject()
    {
        if (!drawGrid || grid == null) return;

        // Allow Scene View and Game Camera
        //if (targetCamera && Camera.current != targetCamera && Camera.current.cameraType != CameraType.SceneView)
            //return;

        if (lineMaterial == null)
            EnsureMaterial();

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.identity);

        DrawGrid();
        if (drawBounds)
            DrawBounds();

        GL.PopMatrix();
    }

    private void DrawGrid()
    {
        Vector2 origin = grid.origin;
        float cell = grid.cellSize;
        int width = grid.width;
        int height = grid.height;

        // Vertical lines
        for (int x = 0; x <= width; x++)
        {
            bool isMajor = (majorEvery > 1) && (x % majorEvery == 0);
            Color c = isMajor ? majorLineColor : lineColor;

            GL.Begin(GL.LINES);
            GL.Color(c);
            float xw = origin.x + x * cell;
            GL.Vertex(new Vector3(xw, origin.y, zOffset));
            GL.Vertex(new Vector3(xw, origin.y + height * cell, zOffset));
            GL.End();
        }

        // Horizontal lines
        for (int y = 0; y <= height; y++)
        {
            bool isMajor = (majorEvery > 1) && (y % majorEvery == 0);
            Color c = isMajor ? majorLineColor : lineColor;

            GL.Begin(GL.LINES);
            GL.Color(c);
            float yw = origin.y + y * cell;
            GL.Vertex(new Vector3(origin.x, yw, zOffset));
            GL.Vertex(new Vector3(origin.x + width * cell, yw, zOffset));
            GL.End();
        }
    }

    private void DrawBounds()
    {
        Vector2 origin = grid.origin;
        float cell = grid.cellSize;
        int width = grid.width;
        int height = grid.height;

        float x0 = origin.x;
        float x1 = origin.x + width * cell;
        float y0 = origin.y;
        float y1 = origin.y + height * cell;

        GL.Begin(GL.LINES);
        GL.Color(majorLineColor);
        GL.Vertex3(x0, y0, zOffset); GL.Vertex3(x1, y0, zOffset);
        GL.Vertex3(x1, y0, zOffset); GL.Vertex3(x1, y1, zOffset);
        GL.Vertex3(x1, y1, zOffset); GL.Vertex3(x0, y1, zOffset);
        GL.Vertex3(x0, y1, zOffset); GL.Vertex3(x0, y0, zOffset);
        GL.End();
    }
}
