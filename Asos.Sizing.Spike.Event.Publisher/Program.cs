using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Asos.Sizing.Spike.Event.Publisher
{
    class Program
    {
        private const string ServiceBusConnectionString =
            "Endpoint=sb://asos-pim-eun-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vDfWcAqx/kwUf1I6LsK6dYVQ56/SswsvAM9EpICv3Lo=";
        const string TopicName = "pim.adapter.spike";
        static ITopicClient topicClient;

        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            for (var i = 0; i < numberOfMessagesToSend; i++)
            {
                try
                {
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    //Console.WriteLine($"Sending message: {messageBody}");

                    await topicClient.SendAsync(message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
                }
            }
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Sending events.");
            const int numberOfMessages = 100;
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            await SendMessagesAsync(numberOfMessages);
            Console.WriteLine("sending completed.");
            Console.ReadKey();
            await topicClient.CloseAsync();
        }
    }
}
