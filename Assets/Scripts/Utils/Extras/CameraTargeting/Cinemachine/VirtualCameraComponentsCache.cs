using Cinemachine;
using System.Collections.Generic;


namespace Honeylab.Utils.CameraTargeting
{
    internal class VirtualCameraComponentsCache
    {
        private readonly Dictionary<CinemachineVirtualCamera, List<CinemachineComponentBase>> _cache =
            new Dictionary<CinemachineVirtualCamera, List<CinemachineComponentBase>>();


        public bool TryGetComponent<T>(CinemachineVirtualCamera camera, out T component)
            where T : CinemachineComponentBase
        {
            if (_cache.TryGetValue(camera, out var components))
            {
                foreach (CinemachineComponentBase componentBase in components)
                {
                    if (componentBase is T tComponent)
                    {
                        component = tComponent;
                        return true;
                    }
                }
            }
            else
            {
                components = new List<CinemachineComponentBase>();
                _cache.Add(camera, components);
            }

            component = camera.GetCinemachineComponent<T>();
            bool wasComponentFound = component != null;
            if (wasComponentFound)
            {
                components.Add(component);
            }

            return wasComponentFound;
        }
    }
}
