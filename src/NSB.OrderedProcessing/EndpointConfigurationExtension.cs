using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;

namespace NSB.OrderedProcessing
{
    public static class EndpointConfigurationExtension
    {
        public static EndpointConfiguration UseOrderedMessaging(this EndpointConfiguration endpointConfig, Action<OrderedProcessingConfig> orderConfig)
        {
            var config = new OrderedProcessingConfig();
            orderConfig(config);
            endpointConfig.GetSettings().Set<OrderedProcessingConfig>(config);
            endpointConfig.EnableFeature<OrderedProcessingFeature>();
            return endpointConfig;
        }

        public static EndpointConfiguration UseOrderedMessaging(this EndpointConfiguration endpointConfig, params Type[] types)
        {
            var config = new OrderedProcessingConfig(types.ToList());
            endpointConfig.GetSettings().Set<OrderedProcessingConfig>(config);
            endpointConfig.EnableFeature<OrderedProcessingFeature>();
            return endpointConfig;
        }
    }
}
