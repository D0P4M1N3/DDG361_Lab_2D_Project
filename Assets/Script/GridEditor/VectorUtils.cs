using UnityEngine;


namespace Builder2D
{
    public static class VectorUtils
    {
        public static float ROTATION_STEP_ANGLE = 90f;

        public static Vector3Int RotateOffset(Vector3Int o, int rot)
        {
            switch (rot % 4)
            {
                case 0: return o;
                case 1: return new Vector3Int(-o.y, o.x, 0);
                case 2: return new Vector3Int(-o.x, -o.y, 0);
                case 3: return new Vector3Int(o.y, -o.x, 0);
                default: return o;
            }
        }


        public static Vector2 RotateOffset(Vector2 o, int rot)
        {
            switch (rot % 4)
            {
                case 0: return o;
                case 1: return new Vector2(-o.y, o.x);
                case 2: return new Vector2(-o.x, -o.y);
                case 3: return new Vector2(o.y, -o.x);
                default: return o;
            }
        }
    }
}
