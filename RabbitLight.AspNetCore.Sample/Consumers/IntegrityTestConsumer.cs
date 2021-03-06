﻿using Microsoft.Extensions.Logging;
using RabbitLight.AspNetCore.Sample.Consumers.Routes;
using RabbitLight.AspNetCore.Sample.Models;
using RabbitLight.Consumer;
using System.IO;

namespace RabbitLight.AspNetCore.Sample.Consumers
{
    [Exchange(Exchanges.Test)]
    public class IntegrityTestConsumer : ConsumerBase
    {
        public const string InputPath = @"C:\Git\RabbitLight\input.txt";
        public const string OutputPath = @"C:\Git\RabbitLight\output.txt";

        private readonly ILogger<IntegrityTestConsumer> _logger;
        private static readonly object _lock = new object();

        public IntegrityTestConsumer(ILogger<IntegrityTestConsumer> logger)
        {
            _logger = logger;
        }

        [Queue(Queues.Integrity, RoutingKeys.Integrity)]
        public void Integrity(MessageContext<TestMessage> context)
        {
            var msg = context.Message();
            lock (_lock)
            {
                _logger.LogInformation("Line: " + msg.Content);
                File.AppendAllText(OutputPath, msg.Content + ",");
            }
        }
    }
}
