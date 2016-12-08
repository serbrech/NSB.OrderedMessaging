using System.Threading.Tasks;
using NServiceBus;

namespace Publisher
{
    public static class PublishExtensions
    {
        public static async Task PublishWithSequenceHeader<TMessage>(this IEndpointInstance endpoint, long seq)
        {
            var pubOptions = new PublishOptions();
            pubOptions.SetHeader("NSB.OrderedProcessing.Sequence", seq.ToString());
            await endpoint.Publish<TMessage>(evt => { }, pubOptions);
        }
    }
}