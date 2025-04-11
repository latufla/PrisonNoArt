using System.Collections.Generic;


namespace Honeylab.Utils.Analytics
{
    public interface ICommonPayloadProvider
    {
        void AddPayload(IDictionary<string, object> commonPayload);
    }
}
