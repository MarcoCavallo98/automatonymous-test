using AutomatonymousTest.Common.Commands;
using AutomatonymousTest.Common.Events;
using MassTransit;

public class DoCConsumer : IConsumer<DoC>
{
    public async Task Consume(ConsumeContext<DoC> context)
    {
        var message = new CDone { Id = context.Message.Id };
        await context.Publish<CDone>(message);
    }
}