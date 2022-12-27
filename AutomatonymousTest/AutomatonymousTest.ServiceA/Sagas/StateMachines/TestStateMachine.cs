using AutomatonymousTest.ServiceA.Sagas.States;
using MassTransit;

namespace AutomatonymousTest.ServiceA.Sagas.StateMachines
{
    public class TestStateMachine : MassTransitStateMachine<TestState>
    {
        #region states
        public State BDone { get; private set; }
        public State CDone { get; private set; }
        #endregion

        #region events
        public Event DoB { get; private set; }
        public Event DoC { get; private set; }
        #endregion

        public TestStateMachine()
        {
            InstanceState(x => x.CurrentState);
        }

    }
}
