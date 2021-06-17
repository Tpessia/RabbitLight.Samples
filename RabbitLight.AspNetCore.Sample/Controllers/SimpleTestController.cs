using Microsoft.AspNetCore.Mvc;
using RabbitLight.AspNetCore.Sample.Consumers.Context;
using RabbitLight.AspNetCore.Sample.Consumers.Routes;
using RabbitLight.AspNetCore.Sample.Models;
using RabbitLight.Publisher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitLight.AspNetCore.Sample.Controllers
{
    [ApiController]
    [Route("simple")]
    public class SimpleTestController : ControllerBase
    {
        private readonly IPublisher _publisher;

        public SimpleTestController(AspNetAppContext busContext)
        {
            _publisher = busContext.Publisher;
        }

        [HttpGet]
        public async Task<string> Test()
        {
            var body = new TestMessage { Content = "Test" };
            await _publisher.PublishJson(Exchanges.Test, RoutingKeys.Test1, body);
            await _publisher.PublishJson(Exchanges.Test, RoutingKeys.Test2, body);
            return "Message published!";
        }

        [HttpGet("batch")]
        public async Task<string> Batch(int size = 500)
        {
            var batch = new List<PublishBatch>();

            for (var i = 0; i < size; i++)
            {
                batch.Add(new PublishBatch(
                    Exchanges.Test,
                    RoutingKeys.Test1,
                    MessageType.Json,
                    new TestMessage { Content = $"Batch: {i}" }
                ));
            }

            await _publisher.PublishBatch(batch);

            return "Messages published!";
        }

        [HttpGet("batch/serially")]
        public async Task<string> BatchSerially(int size = 500)
        {
            var batch = new List<PublishBatch>();

            for (var i = 0; i < size; i++)
            {
                batch.Add(new PublishBatch(
                    Exchanges.Test,
                    RoutingKeys.TestSerially,
                    MessageType.Json,
                    new TestMessage { Content = $"Batch Serially: {i}" }
                ));
            }

            await _publisher.PublishBatch(batch);

            return "Messages published!";
        }

        [HttpGet("error")]
        public async Task<string> Error()
        {
            var body = new TestMessage { Content = "Error test" };
            await _publisher.PublishJson(Exchanges.Test, RoutingKeys.Error, body);
            return "Message published!";
        }

        [HttpGet("infinite")]
        public async Task Infinite()
        {
            var body = new TestMessage { Content = "Infinite test" };
            while (true)
            {
                await _publisher.PublishJson(
                    Exchanges.Test,
                    RoutingKeys.Test1,
                    body
                );
            }
        }
    }
}
