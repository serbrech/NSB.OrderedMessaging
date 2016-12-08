using System;

namespace NSB.OrderedProcessing
{
    internal class OrderedMessagingSequenceMissingException : Exception
    {
        public OrderedMessagingSequenceMissingException(Type messageType):
            base($"The message of type {messageType} has no sequence initilized")
        {
        }
    }
}