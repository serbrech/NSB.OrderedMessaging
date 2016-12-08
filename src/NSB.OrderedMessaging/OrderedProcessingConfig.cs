using System;
using System.Collections.Generic;
using System.Linq;

namespace NSB.OrderedProcessing
{
    public class OrderedProcessingConfig
    {
        public List<Type> MessageTypes { get; set; }

        public OrderedProcessingConfig(List<Type> messageTypes)
        {
            MessageTypes = messageTypes;
        }

        public OrderedProcessingConfig()
        {
            MessageTypes = new List<Type>();
        }
    }
}