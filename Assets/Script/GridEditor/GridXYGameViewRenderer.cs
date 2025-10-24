using UnityEngine;


namespace Builder2D
{
    [ExecuteAlways]
    public class GridXYGameViewRenderer : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private GridSystem _grid;   // assign your GridXY
        [SerializeField] private Camera _targetCamera;      // defaults to Camera.main

        [Header("Appearance")]
        [SerializeField] private Color _lineColor = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField] private Color _majorLineColor = new Color(1f, 1f, 1f, 0.6f);
        [Tooltip("Draw a stronger line every N cells (0/1 = disabled).")]
        [SerializeField] private int _majorEvery = 5;
        [SerializeField] private float _zOffset = 0f;       // push slightly forward/back if needed

        [Header("Fallback (only used if GridXY doesn't expose getters)")]
        [SerializeField] private Vector2 _fallbackOrigin = Vector2.zero;
        [SerializeField] private float _fallbackCellSize = 1f;
        [SerializeField] private int _fallbackWidth = 50;
        [SerializeField] private int _fallbackHeight = 50;

        [Header("Toggle")]
        [SerializeField] private bool _draw = true;
        [SerializeField] private bool _drawBounds = false;

        // Runtime material
        static Material _lineMat;
        static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        static readonly int Cull = Shader.PropertyToID("_Cull");
        static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        static readonly int ZTest = Shader.PropertyToID("_ZTest");

        void OnEnable()
        {
            if (!_targetCamera) _targetCamera = Camera.main;
            EnsureMaterial();
        }

        void EnsureMaterial()
        {
            if (_lineMat != null) return;
            // Hidden/Internal-Colored is perfect for GL lines and supports alpha
            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _lineMat.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMat.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMat.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Off);
            _lineMat.SetInt(ZWrite, 0);
            // Always draw on top so the grid is visible
            _lineMat.SetInt(ZTest, (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        void OnRenderObject()
        {
            if (!_draw) return;
            if (_targetCamera && Camera.current != _targetCamera) return;
            if (_lineMat == null) EnsureMaterial();


            // Read GridXY values (tries public getters first; falls back to serialized overrides)
            Vector2 origin; float cell; int width; int height;
            if (_grid != null)
            {
                origin = _grid.origin;
                cell = _grid.cellSize;
                width = _grid.width;
                height = _grid.height;
            }
            else
            {
                origin = _fallbackOrigin;
                cell = _fallbackCellSize;
                width = Mathf.Max(0, _fallbackWidth);
                height = Mathf.Max(0, _fallbackHeight);
            }

            // Draw
            _lineMat.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);

            // Minor/major lines
            DrawGrid(origin, cell, width, height);

            if (_drawBounds)
                DrawBounds(origin, cell, width, height);

            GL.PopMatrix();
        }

        void DrawGrid(Vector2 origin, float cell, int width, int height)
        {


            // Vertical lines (x changes)
            for (int x = 0; x <= width; x++)
            {
                bool isMajor = (_majorEvery > 1) && (x % _majorEvery == 0);
                var col = isMajor ? _majorLineColor : _lineColor;
                GL.Begin(GL.LINES);
                GL.Color(col);
                float xw = origin.x + x * cell;
                GL.Vertex(new Vector3(xw, origin.y, _zOffset));
                GL.Vertex(new Vector3(xw, origin.y + height * cell, _zOffset));
                GL.End();
            }

            // Horizontal lines (y changes)
            for (int y = 0; y <= height; y++)
            {
                bool isMajor = (_majorEvery > 1) && (y % _majorEvery == 0);
                var col = isMajor ? _majorLineColor : _lineColor;
                GL.Begin(GL.LINES);
                GL.Color(col);
                float yw = origin.y + y * cell;
                GL.Vertex(new Vector3(origin.x, yw, _zOffset));
                GL.Vertex(new Vector3(origin.x + width * cell, yw, _zOffset));
                GL.End();
            }
        }

        void DrawBounds(Vector2 origin, float cell, int width, int height)
        {
            var c = _majorLineColor;
            GL.Begin(GL.LINES);
            GL.Color(c);

            float x0 = origin.x;
            float x1 = origin.x + width * cell;
            float y0 = origin.y;
            float y1 = origin.y + height * cell;

            // Rectangle
            GL.Vertex(new Vector3(x0, y0, _zOffset)); GL.Vertex(new Vector3(x1, y0, _zOffset));
            GL.Vertex(new Vector3(x1, y0, _zOffset)); GL.Vertex(new Vector3(x1, y1, _zOffset));
            GL.Vertex(new Vector3(x1, y1, _zOffset)); GL.Vertex(new Vector3(x0, y1, _zOffset));
            GL.Vertex(new Vector3(x0, y1, _zOffset)); GL.Vertex(new Vector3(x0, y0, _zOffset));

            GL.End();
        }
    }
}

