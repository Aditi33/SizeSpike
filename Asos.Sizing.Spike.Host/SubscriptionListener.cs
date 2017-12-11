using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Asos.Sizing.Spike.Host
{
    public class SubscriptionListener
    {

        static ISubscriptionClient subscriptionClient;
        public void Start(IEnumerable<SubscriptionListenerConfig> subscriptionListenerConfig)
        {
            var subscriptionClient = new SubscriptionClient(
                "Endpoint=sb://asos-pim-eun-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vDfWcAqx/kwUf1I6LsK6dYVQ56/SswsvAM9EpICv3Lo=",
            "pim.adapter.spike",
            "sizing.spike");

            subscriptionClient.RegisterMessageHandler(async (message, token) =>
            {
                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }, ExceptionReceivedHandler);

            //foreach (var config in subscriptionListenerConfig)
            //{


            //    var subscriptionClient = new SubscriptionClient(
            //        config.ServiceBusConnectionString,
            //        config.TopicName, 
            //        config.SubscriptionName);

            //    subscriptionClient.RegisterMessageHandler(async (message, token) =>
            //    {
            //        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            //    }, ExceptionReceivedHandler);
            //}
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is opened in ReceiveMode.PeekLock mode (which is default).
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            //Handle Exceptions
            return Task.CompletedTask;
        }
    }
}
