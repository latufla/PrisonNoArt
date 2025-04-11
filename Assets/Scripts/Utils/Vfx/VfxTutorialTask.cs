using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Honeylab.Gameplay.Vfx
{
    public class VfxTutorialTask : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _root;
        [SerializeField] private Ease _xzEase;
        [SerializeField] private Ease _yEase;
        [SerializeField] private float _outDelay = 0.3f;
        private Vector3 _targetPos;
        public void SetEndPosition(Vector3 targetPos)
        {
            _targetPos = targetPos;
        }
        private void LateUpdate()
        {
            if (!_root.isPlaying) return;
            int particleCount = _root.particleCount;
            if (particleCount == 0)
            {
                return;
            }

            var particlesArray = new NativeArray<ParticleSystem.Particle>(particleCount,
                Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);
            _root.GetParticles(particlesArray);

            Vector3 targetWorld = _targetPos;
            Vector3 targetRootSpace = _root.main.simulationSpace switch
            {
                ParticleSystemSimulationSpace.Local => _root.transform.InverseTransformPoint(targetWorld),
                ParticleSystemSimulationSpace.World => targetWorld,
                var _ => throw new InvalidOperationException()
            };

            for (int i = 0; i < particleCount; i++)
            {
                ParticleSystem.Particle particle = particlesArray[i];
                float t = 1.0f - Mathf.Clamp01(particle.remainingLifetime + _outDelay);
                Vector3 prevParticlePosition = particle.position;
                float xzT = DOVirtual.EasedValue(0.0f, 1.0f, t, _xzEase);
                float yT = DOVirtual.EasedValue(0.0f, 1.0f, t, _yEase);

                Vector3 newParticlePosition = new Vector3(
                    Mathf.Lerp(prevParticlePosition.x, targetRootSpace.x, xzT),
                    Mathf.Lerp(prevParticlePosition.y, targetRootSpace.y, yT),
                    Mathf.Lerp(prevParticlePosition.z, targetRootSpace.z, xzT));
                particle.position = newParticlePosition;

                particlesArray[i] = particle;
            }

            _root.SetParticles(particlesArray);
        }
    }
}
