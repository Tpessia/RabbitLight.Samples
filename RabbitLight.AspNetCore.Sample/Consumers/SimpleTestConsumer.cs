using Microsoft.Extensions.Logging;
using RabbitLight.AspNetCore.Sample.Consumers.Routes;
using RabbitLight.AspNetCore.Sample.Models;
using RabbitLight.Consumer;
using RabbitLight.Exceptions;
using System.Threading.Tasks;

namespace RabbitLight.AspNetCore.Sample.Consumers
{
    [Exchange(Exchanges.Test)]
    public class SimpleTestConsumer : ConsumerBase
    {
        private readonly ILogger<SimpleTestConsumer> _logger;

        public SimpleTestConsumer(ILogger<SimpleTestConsumer> logger)
        {
            _logger = logger;
        }

        [Queue(Queues.Test1, RoutingKeys.Test1)]
        [Queue(Queues.Test2, RoutingKeys.Test2)]
        public async Task Test(MessageContext<TestMessage> context)
        {
            var headers = context.Headers();
            var msg = context.Message();
            _logger.LogInformation($"Message received: {msg.Content}");
            await Task.Delay(500);
        }

        [Queue(Queues.TestSerially, maxChannels: 1, routingKeys: RoutingKeys.TestSerially)]
        public async Task TestSerially(MessageContext<TestMessage> context)
        {
            var headers = context.Headers();
            var msg = context.Message();
            _logger.LogInformation($"Message received: {msg.Content}");
            await Task.Delay(500);
        }

        [Queue(Queues.Error, arguments: "x-dead-letter-exchange: dl.test", routingKeys: RoutingKeys.Error)]
        public void Error(MessageContext<TestMessage> context)
        {
            throw new SerializationException("Error Test");
        }
    }
}
