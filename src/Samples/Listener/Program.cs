using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSB.OrderedProcessing;
using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Logging;
using Shared;

namespace Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        private static async Task AsyncMain()
        {
            Console.Title = "OrderedMessaging.Listener";
            LogManager.Use<DefaultFactory>().Level(LogLevel.Info);
            EndpointConfiguration config = new EndpointConfiguration("ordered.listener");
            config.Conventions().DefiningEventsAs(type => new HashSet<Type> {typeof(IBookSold)}.Contains(type));
            config.SendFailedMessagesTo("ordered.listener.error");
            config.UseOrderedMessaging(typeof(IBookSold));
            config.UseTransport<MsmqTransport>();
            config.UsePersistence<InMemoryPersistence>();
            config.EnableInstallers();
            var endpoint = await Endpoint.Start(config).ConfigureAwait(false);
            try
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            finally
            {
                await endpoint.Stop()
                    .ConfigureAwait(false);
            }
        }
    }

    public class BookSoldHandler : IHandleMessages<IBookSold>
    {
        static ILog log = LogManager.GetLogger<BookSoldHandler>();
        public Task Handle(IBookSold message, IMessageHandlerContext context)
        {
            log.Info($"Received Book Sold {context.MessageHeaders["NSB.OrderedProcessing.Sequence"]}");
            return Task.CompletedTask;
        }
    }
}
