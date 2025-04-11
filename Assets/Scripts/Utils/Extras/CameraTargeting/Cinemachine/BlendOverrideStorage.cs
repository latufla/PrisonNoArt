using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.CameraTargeting
{
    internal class BlendOverrideStorage : IDisposable
    {
        private readonly Dictionary<ICinemachineCamera, CameraTargetingOverrides> _overridesByCamera =
            new Dictionary<ICinemachineCamera, CameraTargetingOverrides>();


        public void Run() => CinemachineCore.GetBlendOverride = OverrideBlendIfNeeded;


        public void Add(CinemachineVirtualCamera virtualCamera, CameraTargetingOverrides overrides) =>
            _overridesByCamera.Add(virtualCamera, overrides);


        public void Remove(CinemachineVirtualCamera virtualCamera) => _overridesByCamera.Remove(virtualCamera);


        public void Dispose() => CinemachineCore.GetBlendOverride = null;


        private CinemachineBlendDefinition OverrideBlendIfNeeded(ICinemachineCamera fromVirtualCamera,
            ICinemachineCamera toVirtualCamera,
            CinemachineBlendDefinition defaultBlend,
            MonoBehaviour owner)
        {
            if (!_overridesByCamera.TryGetValue(toVirtualCamera, out CameraTargetingOverrides overrides))
            {
                return defaultBlend;
            }

            CinemachineBlendDefinition blendDefinition = defaultBlend;

            if (overrides.TryGetBlendDuration(out float duration))
            {
                blendDefinition.m_Time = duration;
            }

            if (overrides.TryGetBlendEaseCurve(out AnimationCurve easeCurve))
            {
                blendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
                blendDefinition.m_CustomCurve = easeCurve;
            }

            return blendDefinition;
        }
    }
}
