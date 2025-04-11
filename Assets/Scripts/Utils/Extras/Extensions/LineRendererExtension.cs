using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class LineRendererExtension
    {
        public static void SetPointsFromEnumerable(this LineRenderer r, IEnumerable<Vector3> points)
        {
            int pointsSet = 0;
            foreach (Vector3 point in points)
            {
                if (r.positionCount <= pointsSet)
                {
                    r.positionCount++;
                }

                r.SetPosition(pointsSet, point);
                pointsSet++;
            }

            r.positionCount = Mathf.Min(r.positionCount, pointsSet);
        }
    }
}
