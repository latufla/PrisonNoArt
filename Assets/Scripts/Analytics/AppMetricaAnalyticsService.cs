using Honeylab.Utils.Analytics;
using System.Collections.Generic;


namespace Honeylab.Analytics
{
    public class AppMetricaAnalyticsService : IAnalyticsService
    {
        public void ReportEvent(string key, IReadOnlyDictionary<string, object> message)
        {
            var poolMessage = DictionaryPool<string, object>.Pop();
            if (message != null)
            {
                foreach (var messageKvp in message)
                {
                    poolMessage.Add(messageKvp.Key, messageKvp.Value);
                }
            }

            AppMetrica.Instance.ReportEvent(key, poolMessage);

            DictionaryPool<string, object>.Push(poolMessage);
        }


        public void SendEventsBuffer() => AppMetrica.Instance.SendEventsBuffer();
    }
}
