using AutomatonymousTest.Common.Commands;
using AutomatonymousTest.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatonymousTest.ServiceB
{
    public class DoBConsumer : IConsumer<DoB>
    {
        public async Task Consume(ConsumeContext<DoB> context)
        {
            var message = new BDone { Id = context.Message.Id };
            await context.Publish<BDone>(message);
        }
    }
}
