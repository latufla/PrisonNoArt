using System.Collections.Generic;


namespace Honeylab.Utils.Analytics
{
    public interface IAnalyticsService
    {
        void ReportEvent(string key, IReadOnlyDictionary<string, object> message);
        void SendEventsBuffer();
    }
}
