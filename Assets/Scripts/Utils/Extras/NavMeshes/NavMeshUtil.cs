using System;
using UnityEngine;
using UnityEngine.AI;


namespace Honeylab.Utils.NavMeshes
{
    public static class NavMeshUtil
    {
        public static bool TryCalculatePathLength(Vector3 sourcePosition,
            Vector3 targetPosition,
            float acceptableDistanceError,
            Vector3[] cornersBuffer,
            NavMeshPath pathBuffer,
            out float pathLength,
            int areaMask = NavMesh.AllAreas)
        {
            if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, acceptableDistanceError, areaMask))
            {
                pathLength = default;
                return false;
            }

            if (!NavMesh.CalculatePath(sourcePosition, hit.position, areaMask, pathBuffer))
            {
                pathLength = default;
                return false;
            }

            int cornersCount = pathBuffer.GetCornersNonAlloc(cornersBuffer);
            if (cornersCount >= cornersBuffer.Length)
            {
                throw new InvalidOperationException("Corners buffer size reached.");
            }

            if (Vector3.Distance(cornersBuffer[cornersCount - 1], targetPosition) > acceptableDistanceError)
            {
                pathLength = default;
                return false;
            }

            pathLength = 0.0f;
            Vector3 previousCorner = sourcePosition;
            for (int i = 0; i < cornersCount; i++)
            {
                Vector3 currentCorner = cornersBuffer[i];
                float segmentLength = Vector3.Distance(currentCorner, previousCorner);
                pathLength += segmentLength;
                previousCorner = currentCorner;
            }

            return true;
        }
    }
}
