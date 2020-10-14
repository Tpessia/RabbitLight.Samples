using RabbitLight.Config;
using RabbitLight.Context;
using System;

namespace RabbitLight.ConsoleApp.Sample.Consumers.Context
{
    public class ConsoleAppContext : RabbitLightContext
    {
        public ConsoleAppContext(IServiceProvider sp, ContextConfig config) : base(sp, config)
        {
        }
    }
}
