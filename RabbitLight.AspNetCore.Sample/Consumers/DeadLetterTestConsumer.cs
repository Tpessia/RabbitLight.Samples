using Microsoft.Extensions.Logging;
using RabbitLight.AspNetCore.Sample.Consumers.Routes;
using RabbitLight.AspNetCore.Sample.Models;
using RabbitLight.Consumer;
using System.Threading.Tasks;

namespace RabbitLight.AspNetCore.Sample.Consumers
{
    [Exchange(Exchanges.DLTest)]
    public class DeadLetterTestConsumer : ConsumerBase
    {
        private readonly ILogger<DeadLetterTestConsumer> _logger;

        public DeadLetterTestConsumer(ILogger<DeadLetterTestConsumer> logger)
        {
            _logger = logger;
        }

        [Queue(Queues.DLTest, RoutingKeys.Error)]
        public async Task Test1(MessageContext<TestMessage> context)
        {
            var msg = context.Message();
            _logger.LogInformation($"Dead Letter Message received: {msg.Content}");
            await Task.Delay(500);
        }
    }
}
