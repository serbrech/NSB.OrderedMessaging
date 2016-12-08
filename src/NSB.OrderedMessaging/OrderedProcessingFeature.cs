using System.CodeDom;
using System.Linq;
using NServiceBus.Features;

namespace NSB.OrderedProcessing
{
    public class OrderedProcessingFeature : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var settings = context.Settings.GetOrDefault<OrderedProcessingConfig>();
            context.Pipeline.Register(new OrderedIncomingMessageBehavior(settings), "Ensures messages will be processed in order using sequence numbers");
            context.Pipeline.Register(new OrderedOutgoingMessageBehavior(settings), "Ensures messages will be sent in order using sequence numbers");
        }
    }
}
