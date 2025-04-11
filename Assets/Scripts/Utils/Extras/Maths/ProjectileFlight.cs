using UnityEngine;


namespace Honeylab.Utils.Maths
{
    /// <summary>
    /// y = at^2 + bt + c
    /// t = 0 - start
    /// t = tv - vertex
    /// t = 1 - end
    /// -b / 2a = tv
    ///
    /// x, z - linear motion
    ///
    /// t = 0 => c = start
    /// t = 1 => a + b + c = a + b + start = end
    /// t = tv => a*tv^2 + b*tv + c = vertex
    ///
    /// t = tv:
    ///     -(end - start - a)^2 + start = vertex
    ///     a = end + start - 2 * vertex ± sqrt(D)
    ///     D = 16(vertex - end)(vertex - start)
    ///     sqrt(D) = 4 * sqrt((vertex - end)(vertex - start))
    /// </summary>
    public readonly struct ProjectileFlight
    {
        private readonly Vector3 _start;
        private readonly Vector3 _end;
        private readonly float _a;
        private readonly float _b;


        public ProjectileFlight(Vector3 start, Vector3 end, float vertexRaise)
        {
            _start = start;
            _end = end;
            Vector3 vertex = 0.5f * (start + end);
            vertex.y = Mathf.Max(start.y, end.y) + vertexRaise;

            float underSqrtExpr = vertex.y * vertex.y -
                vertex.y * end.y -
                vertex.y * start.y +
                end.y * start.y;

            float sqrtD = 4 * Mathf.Sqrt(underSqrtExpr);

            _a = CalculateA(start.y, end.y, vertex.y, sqrtD, -1.0f);
            _b = end.y - _a - start.y;


            static float CalculateA(float start, float end, float vertex, float sqrtD, float sign)
            {
                return end + start - 2 * vertex + sign * sqrtD / 2;
            }
        }


        public Vector3 Evaluate(float t)
        {
            float c = _start.y;
            float y = t * t * _a + t * _b + c;

            Vector3 xz = Vector3.LerpUnclamped(_start, _end, t);
            xz.y = 0;

            return y * Vector3.up + xz;
        }
    }
}
