using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSB.OrderedProcessing.Exceptions;
using NServiceBus.Logging;
using NServiceBus.Pipeline;

namespace NSB.OrderedProcessing
{
    public class OrderedIncomingMessageBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        static ILog _log = LogManager.GetLogger<OrderedIncomingMessageBehavior>();
        private readonly HashSet<Type> _messageTypes;

        public OrderedIncomingMessageBehavior(OrderedProcessingConfig config)
        {
            _messageTypes = new HashSet<Type>(config.MessageTypes);
            foreach (var messageType in _messageTypes)
            {
                State.Sequences.TryAdd(messageType, 0);
            }
        }

        //We'll need to persist this somewhere anyway as part of the tx.
        //means we'll need to let user provide an adapter.
        private static readonly OrderedProcessingState State = new OrderedProcessingState();

        public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            if (_messageTypes.Contains(context.Message.MessageType))
            {
                long messageSequence;
                if (!long.TryParse(context.Headers["NSB.OrderedProcessing.Sequence"], out messageSequence))
                {
                    throw new OrderedMessagingSequenceMissingException(context.Message.MessageType);
                }

                //it's ok to not do it atomically, because if I'm the next message to be processed, then no other message can possibly increment the counter.
                ValidateSequenceNumber(messageSequence, context.Message.MessageType);
                State.Sequences.AddOrUpdate(context.Message.MessageType, type => 1, (type, l) => messageSequence);

            }
            return next();
        }

        void ValidateSequenceNumber(long messageSequence, Type messageType)
        {
            //How to handle first message? Where do we start the sequence?
            //Set state on setup to 0, so the key is present. next message must be 1.
            //How do we force a gap if we need it? user can change the sequence in the header in the error queue..
            //set a flag in the header?
            //set the last sequence in the persisted state
            long lastMessageSequence;
            if (State.Sequences.TryGetValue(messageType, out lastMessageSequence))
            {
                //Happy path. no gap detected.
                if (IsConsecutiveSequence(lastMessageSequence, messageSequence))
                {
                    return;
                }

                //We are behing. gap detected
                if (MessageSequenceBehing(lastMessageSequence, messageSequence))
                {
                    throw new OutOfOrderMessagingException(messageType, messageSequence,
                        lastMessageSequence);
                }

                //This is all the other cases
                //we are ahead already. discard.
                //if (lastMessageSequence >= messageSequence)
                //{
                throw new OrderedMessagingSequenceAheadException(messageType, messageSequence,
                    lastMessageSequence);
                //}

            }

            throw new InvalidOperationException($"There is no current sequence in the receiver for message type {messageType}");
        }

        private static bool MessageSequenceBehing(long lastMessageSequence, long messageSequence)
        {
            return lastMessageSequence + 1 < messageSequence;
        }

        private static bool IsConsecutiveSequence(long lastMessageSequence, long messageSequence)
        {
            return lastMessageSequence + 1 == messageSequence;
        }
    }
}