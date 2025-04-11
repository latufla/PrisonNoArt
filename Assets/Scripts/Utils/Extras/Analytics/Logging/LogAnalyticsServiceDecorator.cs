using Honeylab.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Honeylab.Utils.Analytics
{
    public class LogAnalyticsServiceDecorator : IAnalyticsService
    {
        private readonly IAnalyticsService _analyticsService;


        public LogAnalyticsServiceDecorator(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }


        public void ReportEvent(string key, IReadOnlyDictionary<string, object> message)
        {
            this.SelfLog($"{key} -> {(message != null ? DictionaryToReadableString(message) : string.Empty)}");
            _analyticsService.ReportEvent(key, message);
        }


        public void SendEventsBuffer()
        {
            this.SelfLog(nameof(SendEventsBuffer));
            _analyticsService.SendEventsBuffer();
        }


        private static string DictionaryToReadableString(IReadOnlyDictionary<string, object> dictionary)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                string keyStr = ObjectToReadableString(kvp.Key);
                string valueStr = ObjectToReadableString(kvp.Value);
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.AppendFormat("{0} : {1}", keyStr, valueStr);
            }

            return $"{{{sb}}}";
        }


        private static string EnumerableToReadableString(IEnumerable enumerable)
        {
            StringBuilder sb = new StringBuilder();

            foreach (object item in enumerable)
            {
                string itemStr = ObjectToReadableString(item);
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(itemStr);
            }

            return $"[{sb}]";
        }


        private static string ObjectToReadableString(object obj)
        {
            if (obj is IReadOnlyDictionary<string, object> dictionary)
            {
                return DictionaryToReadableString(dictionary);
            }

            if (obj is string objStr)
            {
                return objStr;
            }

            if (obj is IEnumerable enumerable)
            {
                return EnumerableToReadableString(enumerable);
            }

            return obj.ToString();
        }
    }
}
