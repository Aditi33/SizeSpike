using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Asos.Sizing.Spike.Host
{
    class Program
    {
        private const string ServiceBusConnectionString =
                "Endpoint=sb://asos-pim-eun-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vDfWcAqx/kwUf1I6LsK6dYVQ56/SswsvAM9EpICv3Lo=";
        const string TopicName = "pim.adapter.spike";
        const string SubscriptionName = "sizing.spike1";
        static ISubscriptionClient subscriptionClient;
            
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
            ////Console.WriteLine("Sizing spike has started.");

            ////var config = new SpikeConfiguration();

            ////var subscriptionListener = new SubscriptionListener();

            ////subscriptionListener.Start(config.SubscriptionListeners);

            ////Console.WriteLine("Sizing spike has completed processing.");
            Console.ReadLine();
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,

                AutoComplete = false
            };

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        

        static async Task MainAsync()
        {
            
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press any key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");

            RegisterOnMessageHandlerAndReceiveMessages();

            

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
            
        }
    }
}
