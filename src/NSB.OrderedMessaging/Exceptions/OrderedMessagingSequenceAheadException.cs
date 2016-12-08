using System;

namespace NSB.OrderedProcessing.Exceptions
{
    internal class OrderedMessagingSequenceAheadException : Exception
    {
        public OrderedMessagingSequenceAheadException(Type messageType, long messageSequence, long lastMessageSequence)
            : base($"Received messagetype {messageType} with sequence {messageSequence}, but last processed message sequence was {lastMessageSequence}\n" +
                   "Message should not be processed.") { }
    }
}