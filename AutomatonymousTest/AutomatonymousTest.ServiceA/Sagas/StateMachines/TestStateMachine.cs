using AutomatonymousTest.Common.Commands;
using AutomatonymousTest.Common.Events;
using AutomatonymousTest.ServiceA.Sagas.States;
using MassTransit;

namespace AutomatonymousTest.ServiceA.Sagas.StateMachines
{
    public class TestStateMachine : MassTransitStateMachine<TestState>
    {
        #region states
        public State DoingB { get; private set; }
        public State DoingC { get; private set; }
        #endregion

        #region events/commands
        public Event<StartTestSaga> StartTestSagaCommand { get; private set; }
        public Event<BDone> BDoneEvent { get; private set; }
        public Event<CDone> CDoneEvent { get; private set; }
        #endregion

        public TestStateMachine()
        {
            InstanceState(x => x.CurrentState);


            Event(() => StartTestSagaCommand, x => x.CorrelateById(context => context.Message.Id));
            Event(() => BDoneEvent, x => x.CorrelateById(context => context.Message.ItemId));
            Event(() => CDoneEvent, x => x.CorrelateById(context => context.Message.ItemId));

            Initially(
                When(StartTestSagaCommand)
                    .Then(x =>
                    {
                        x.Saga.LastStateUpdate = DateTime.UtcNow;
                        x.Saga.Name = x.Message.Name;
                    })
                    .Send(new Uri("queue:service-b"), ctx => new DoB { ItemId = ctx.Saga.CorrelationId })
                    .TransitionTo(DoingB));

            During(DoingB,
                When(BDoneEvent)
                    .Then(x => x.Saga.LastStateUpdate = DateTime.UtcNow)
                    .Send(new Uri("queue:service-c"), ctx => new DoC { ItemId = ctx.Saga.CorrelationId })
                    .TransitionTo(DoingC));

            During(DoingC,
                When(CDoneEvent)
                    .Then(x => x.Saga.LastStateUpdate = DateTime.UtcNow)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

    }
}
