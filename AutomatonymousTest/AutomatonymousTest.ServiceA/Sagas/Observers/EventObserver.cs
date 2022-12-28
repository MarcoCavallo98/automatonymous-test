using AutomatonymousTest.ServiceA.Sagas.States;
using MassTransit;

namespace AutomatonymousTest.ServiceA.Sagas.Observers
{
    public class EventObserver : IEventObserver<TestState>
    {
        private readonly ILogger _logger;

        public EventObserver(ILogger logger)
        {
            _logger = logger;
        }

        public Task ExecuteFault(BehaviorContext<TestState> context, Exception exception)
        {
            _logger.LogWarning(exception, exception.Message);
            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(BehaviorContext<TestState, T> context, Exception exception) where T : class
        {
            _logger.LogWarning(exception, exception.Message);
            return Task.CompletedTask;
        }

        public Task PostExecute(BehaviorContext<TestState> context)
        {
            _logger.LogDebug("Handled event {Name}", context.Event.Name);
            return Task.CompletedTask;
        }

        public Task PostExecute<T>(BehaviorContext<TestState, T> context) where T : class
        {
            _logger.LogDebug("Handled event {Name}", context.Event.Name);
            return Task.CompletedTask;
        }

        public Task PreExecute(BehaviorContext<TestState> context)
        {
            _logger.LogDebug("Received event {Name}", context.Event.Name);
            return Task.CompletedTask;
        }

        public Task PreExecute<T>(BehaviorContext<TestState, T> context) where T : class
        {
            _logger.LogDebug("Received event {Name}", context.Event.Name);
            return Task.CompletedTask;
        }
    }
}
