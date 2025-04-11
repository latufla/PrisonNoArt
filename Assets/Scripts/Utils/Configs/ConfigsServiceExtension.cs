using System;


namespace Honeylab.Utils.Configs
{
    public static class ConfigsServiceExtension
    {
        public static T Get<T>(this IConfigsService service, string configId)
        {
            if (service.TryGet(configId, out T config))
            {
                return config;
            }

            throw new ArgumentException($"Can not resolve ID = {configId}", nameof(configId));
        }
    }
}
