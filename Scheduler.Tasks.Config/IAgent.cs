namespace Scheduler
{
    public interface IAgent
    {
        #region Properties

        Guid AgentId { get; }
        TaskConfig Config { get; }

        #endregion Properties

        #region Methods

        void Init(Guid agentId, TaskConfig taskConfig);

        void Evaluate();

        void Terminate();

        #endregion Methods
    }
}