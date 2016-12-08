using System;
using System.Collections.Concurrent;

namespace NSB.OrderedProcessing
{
    internal class OrderedProcessingState
    {
        public OrderedProcessingState()
        {
            Sequences = new ConcurrentDictionary<Type, long>();
        }
        public ConcurrentDictionary<Type, long> Sequences { get; }
    }
}