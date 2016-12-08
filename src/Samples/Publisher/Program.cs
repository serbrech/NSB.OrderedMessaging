using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSB.OrderedProcessing;
using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;
using Shared;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        private static async Task AsyncMain()
        {
            Console.Title = "Ordered.Publisher";
            EndpointConfiguration config = new EndpointConfiguration("ordered.publisher");
            //We set headers manually, no need to enable the behavior...
            //config.UseOrderedMessaging(typeof(IBookSold));
            config.EnableInstallers();
            config.Conventions().DefiningEventsAs(type => new HashSet<Type> {typeof(IBookSold)}.Contains(type));
            config.SendFailedMessagesTo("ordered.publisher.error");
            config.UseTransport<MsmqTransport>();
            config.UsePersistence<InMemoryPersistence>();
            var endpoint = await Endpoint.Start(config);
            try
            {
                Console.WriteLine("Enter seq number to publish. non numeric to exit");
                while (true)
                {
                    long seq;
                    var key = Console.ReadKey();
                    if (long.TryParse(key.KeyChar.ToString(), out seq))
                    {
                        await endpoint.PublishWithSequenceHeader<IBookSold>(seq);
                        Console.WriteLine();
                        Console.WriteLine($"published event with sequence {seq}");
                    }
                    else
                    {
                        break;
                    }
                }
                Console.WriteLine("exiting the publisher");
            }
            finally
            {
                await endpoint.Stop()
                    .ConfigureAwait(false);
            }
        }
    }
}
