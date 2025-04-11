using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class GizmosUtil
    {
        public static void DrawForwardArrow()
        {
            const float thickness = 0.2f;
            const float baseLength = 1.0f;
            const float sideLength = 0.6f;
            const float sideAngle = 45.0f;

            Matrix4x4 baseMatrix = Gizmos.matrix;
            Gizmos.matrix = baseMatrix *
                Matrix4x4.Translate(baseLength * Vector3.forward) *
                Matrix4x4.Rotate(Quaternion.Euler(0.0f, 180.0f - sideAngle, 0.0f));
            Gizmos.DrawCube(sideLength * 0.5f * Vector3.forward, new Vector3(thickness, thickness, sideLength));

            Gizmos.matrix = baseMatrix *
                Matrix4x4.Translate(baseLength * Vector3.forward) *
                Matrix4x4.Rotate(Quaternion.Euler(0.0f, sideAngle - 180.0f, 0.0f));
            Gizmos.DrawCube(sideLength * 0.5f * Vector3.forward, new Vector3(thickness, thickness, sideLength));

            Gizmos.matrix = baseMatrix;
            Gizmos.DrawCube(baseLength * 0.5f * Vector3.forward, new Vector3(thickness, thickness, baseLength));
        }
    }
}
