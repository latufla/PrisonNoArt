using System.Collections.Generic;


namespace Honeylab.Utils.Analytics
{
    public class CommonPayloadProvidersStorage
    {
        private readonly List<ICommonPayloadProvider> _payloadProviders = new List<ICommonPayloadProvider>();


        public void AddPayloadProvider(ICommonPayloadProvider payloadProvider) =>
            _payloadProviders.Add(payloadProvider);


        public List<ICommonPayloadProvider>.Enumerator GetEnumerator() => _payloadProviders.GetEnumerator();
    }
}
