using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace NSB.OrderedProcessing
{
    public class OrderedOutgoingMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
    {
        private readonly HashSet<Type> _messageTypes;

        public OrderedOutgoingMessageBehavior(OrderedProcessingConfig config)
        {
            _messageTypes = new HashSet<Type>(config.MessageTypes);
        }
        private static readonly OrderedProcessingState State = new OrderedProcessingState();
        public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            if (_messageTypes.Contains(context.Message.MessageType))
            {
                //start at 1 or increment.
                var currentMessageSequence = State.Sequences.AddOrUpdate(context.Message.MessageType, type => 1, (type, l) => l++);
                context.Headers["NSB.OrderedProcessing.Sequence"] = currentMessageSequence.ToString();
            }
            return next();
        }
    }
}