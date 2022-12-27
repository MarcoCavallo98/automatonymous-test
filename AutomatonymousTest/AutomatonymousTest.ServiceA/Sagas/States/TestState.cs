using MassTransit;

namespace AutomatonymousTest.ServiceA.Sagas.States
{
    public class TestState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } // For a real world app, we should use int, not string, in order to take less space
        public DateTime? LastStateUpdate { get; set; }
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
    }
}
