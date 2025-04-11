using System.Collections.Generic;


namespace Honeylab.Utils.Analytics
{
    public sealed class CommonPayloadAnalyticsServiceDecorator : IAnalyticsService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly CommonPayloadProvidersStorage _commonPayloadProvidersStorage;


        public CommonPayloadAnalyticsServiceDecorator(IAnalyticsService analyticsService,
            CommonPayloadProvidersStorage commonPayloadProvidersStorage)
        {
            _analyticsService = analyticsService;
            _commonPayloadProvidersStorage = commonPayloadProvidersStorage;
        }


        public void ReportEvent(string key, IReadOnlyDictionary<string, object> message)
        {
            var commonPayload = DictionaryPool<string, object>.Pop();
            foreach (ICommonPayloadProvider payloadProvider in _commonPayloadProvidersStorage)
            {
                payloadProvider.AddPayload(commonPayload);
            }

            var messageWithCommonPayload = DictionaryPool<string, object>.Pop();
            if (message != null)
            {
                foreach (var messageKvp in message)
                {
                    messageWithCommonPayload.Add(messageKvp.Key, messageKvp.Value);
                }
            }

            messageWithCommonPayload.Add(CommonAnalytics.Messages.CommonPayload, commonPayload);
            _analyticsService.ReportEvent(key, messageWithCommonPayload);

            DictionaryPool<string, object>.Push(commonPayload);
            DictionaryPool<string, object>.Push(messageWithCommonPayload);
        }


        public void SendEventsBuffer() => _analyticsService.SendEventsBuffer();
    }
}
