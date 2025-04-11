using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;


namespace Honeylab.Gameplay.Creatures
{
    public class CreatureMotion : WorldObjectComponentBase
    {
        [SerializeField] private NavMeshAgent _agent;

        private CancellationTokenSource _movementCts;


        public NavMeshAgent Agent => _agent;


        public UniTask MoveToTargetAsync(Transform target) => MoveToTargetAsync(target.position);


        public async UniTask MoveToTargetAsync(Vector3 destination, List<EnemyFlow> enemies = null, float nearbyAgentDistance = 0)
        {
            _movementCts?.CancelThenDispose();
            _movementCts = null;

            _agent.updateRotation = true;
            _agent.isStopped = false;
            _agent.SetDestination(destination);

            if (_agent.IsDestinationReached())
            {
                return;
            }

            _movementCts = CancellationTokenSource.CreateLinkedTokenSource(_agent.GetCancellationTokenOnDestroy());
            await UniTask.WaitUntil(() =>
            {
                (bool destinationReached, bool enemyFound, Vector3 enemyPos) = _agent.IsDestinationReached(enemies, nearbyAgentDistance);

                if (destinationReached)
                {
                    return true;
                }

                if (enemyFound)
                {
                    Vector3 dirrectionAway = transform.position - enemyPos;
                    Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * nearbyAgentDistance;
                    Vector3 newDistination = transform.position + dirrectionAway.normalized * nearbyAgentDistance + randomOffset;

                    if (NavMesh.SamplePosition(newDistination, out NavMeshHit navMeshHit, nearbyAgentDistance, NavMesh.AllAreas))
                    {
                        _agent.SetDestination(navMeshHit.position);
                    }
                    else
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }, cancellationToken: _movementCts.Token);
            _movementCts.CancelThenDispose();
            _movementCts = null;
        }


        public void StopMoveToTarget()
        {
            _movementCts?.CancelThenDispose();
            _movementCts = null;

            _agent.isStopped = true;
            _agent.ResetPath();
        }


        public bool IsDestinationReached(Vector3 destination)
        {
            Vector3 position = _agent.transform.position;
            position.y = destination.y;
            bool result = Vector3.Distance(position, destination) < 0.01f;
            return result;
        }


        public float GetRemainDistance()
        {
            if (_agent.pathPending ||
                _agent.pathStatus == NavMeshPathStatus.PathInvalid ||
                _agent.path.corners.Length == 0)
            {
                return -1f;
            }

            float distance = 0.0f;
            for (int i = 0; i < _agent.path.corners.Length - 1; ++i)
            {
                distance += Vector3.Distance(_agent.path.corners[i], _agent.path.corners[i + 1]);
            }

            return distance;
        }


        public void RotateTowards(Vector3 direction)
        {
            _agent.updateRotation = false;
            _agent.transform.rotation = Quaternion.LookRotation(direction);
        }


        public void LookAt(Transform target)
        {
            _agent.updateRotation = false;
            _agent.transform.LookAt(target);
        }


        public void SetSpeed(float speed) => _agent.speed = speed;
        public void SetRotateSpeed(float rotateSpeed) => _agent.angularSpeed = rotateSpeed;
        public void SetAcceleration(float acceleration) => _agent.acceleration = acceleration;


        protected override void OnInit()
        {
            _agent.enabled = true;

            _agent.isStopped = true;
            _agent.ResetPath();
        }


        protected override void OnClear()
        {
            _movementCts?.CancelThenDispose();
            _movementCts = null;
        }


        public bool IsMoving() => _agent.hasPath && !_agent.isStopped && !_agent.pathPending;
    }
}
