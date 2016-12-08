using System;

namespace NSB.OrderedProcessing.Exceptions
{
    internal class OutOfOrderMessagingException : Exception
    {
       
        public OutOfOrderMessagingException(Type messageType, long messageSequence, long lastMessageSequence)
            :base($"Received messagetype {messageType} with sequence {messageSequence}, but last processed message sequence was {lastMessageSequence}.\n" +
                   "Waiting for missing sequence. Message will be retried.")
        {
        }
    }
}