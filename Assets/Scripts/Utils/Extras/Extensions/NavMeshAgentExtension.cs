using Honeylab.Gameplay.Creatures;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Honeylab.Utils.Extensions
{
    public static class NavMeshAgentExtension
    {
        public static bool IsDestinationReached(this NavMeshAgent agent, float remainingDistanceThreshold = 0.0f)
        {
            float remainingDistanceNow = agent.remainingDistance;
            return !agent.pathPending
                && !float.IsPositiveInfinity(remainingDistanceNow)
                && agent.pathStatus == NavMeshPathStatus.PathComplete
                && remainingDistanceNow <= remainingDistanceThreshold;
        }

        public static (bool destinationReached, bool enemyFound, Vector3 enemyPos) IsDestinationReached(this NavMeshAgent agent, List<EnemyFlow> enemies, float nearbyAgentDistance = 0f, float remainingDistanceThreshold = 0.0f)
        {
            float remainingDistanceNow = agent.remainingDistance;
            bool distinationReached = !agent.pathPending
                && !float.IsPositiveInfinity(remainingDistanceNow)
                && agent.pathStatus == NavMeshPathStatus.PathComplete
                && remainingDistanceNow <= remainingDistanceThreshold;

            if (distinationReached)
                return (true, false, Vector3.zero);

            if (enemies == null)
                return (distinationReached, false, Vector3.zero);

            Vector3 distination = agent.transform.position;

            foreach (var enemy in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, distination) <= nearbyAgentDistance)
                {
                    return (false, true, enemy.transform.position);
                }
            }

            return (false, false, Vector3.zero);
        }
    }
}
