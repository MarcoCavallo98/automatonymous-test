using AutomatonymousTest.Common.Commands;
using AutomatonymousTest.Common.Events;
using AutomatonymousTest.ServiceA.Sagas.Observers;
using AutomatonymousTest.ServiceA.Sagas.States;
using MassTransit;

namespace AutomatonymousTest.ServiceA.Sagas.StateMachines
{
    public class TestStateMachine : MassTransitStateMachine<TestState>
    {
        private readonly ILogger<TestStateMachine> _logger;

        #region states
        public State DoingB { get; private set; }
        public State DoingC { get; private set; }
        #endregion

        #region events/commands
        public Event<StartTestSaga> StartTestSagaCommand { get; private set; }
        public Event<BDone> BDoneEvent { get; private set; }
        public Event<CDone> CDoneEvent { get; private set; }
        #endregion

        public TestStateMachine(ILogger<TestStateMachine> logger)
        {
            _logger = logger;

            ConnectEventObserver(new EventObserver(logger));

            InstanceState(x => x.CurrentState);


            Event(() => StartTestSagaCommand, e => e.CorrelateById(context => context.Message.Id));
            Event(() => BDoneEvent, e => e.CorrelateById(context => context.Message.Id));
            Event(() => CDoneEvent, e => e.CorrelateById(context => context.Message.Id));

            Initially(
                When(StartTestSagaCommand)
                    .Then(x =>
                    {
                        x.Saga.LastStateUpdate = DateTime.UtcNow;
                        x.Saga.Name = $"Test_{x.Message.Id}";
                    })
                    .TransitionTo(DoingB)
                    .Send(new Uri("queue:service-b"), ctx => new DoB { Id = ctx.Saga.CorrelationId }));

            During(DoingB,
                When(BDoneEvent)
                    .Then(x => 
                    { 
                        x.Saga.LastStateUpdate = DateTime.UtcNow;
                        _logger.LogInformation("Moving to DoingC state"); 
                    })
                    .TransitionTo(DoingC)
                    .Send(new Uri("queue:service-c"), ctx => new DoC { Id = ctx.Saga.CorrelationId }));

            During(DoingC,
                When(CDoneEvent)
                    .Then(x =>
                    {
                        x.Saga.LastStateUpdate = DateTime.UtcNow;
                        _logger.LogInformation("Completing SAGA {Name}", x.Saga.Name);
                    })
                    .Finalize());

            SetCompletedWhenFinalized();
        }

    }
}
