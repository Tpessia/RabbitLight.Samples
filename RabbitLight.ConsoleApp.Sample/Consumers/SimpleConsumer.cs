using RabbitLight.ConsoleApp.Sample.Consumers.Routes;
using RabbitLight.Consumer;
using System;

namespace RabbitLight.ConsoleApp.Sample.Consumers
{
    [Exchange(Exchanges.TestExchange)]
    public class SimpleConsumer : ConsumerBase
    {
        [Queue(Queues.TestQueue)]
        public void Test(MessageContext<string> context)
        {
            var msg = context.MessageAsString();
            Console.WriteLine($"Message received: {msg}");
        }
    }
}
