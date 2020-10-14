using RabbitLight.Config;
using RabbitLight.Context;
using System;

namespace RabbitLight.AspNetCore.Sample.Consumers.Context
{
    public class AspNetAppContext : RabbitLightContext
    {
        public AspNetAppContext(IServiceProvider sp, ContextConfig config) : base(sp, config)
        {
        }
    }
}
