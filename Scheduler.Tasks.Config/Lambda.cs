namespace Scheduler.Core
{
    public class Lambda : IAgent
    {
        #region Properties

        public Guid AgentId { get; private set; }
        public TaskConfig Config { get; private set; }

        #endregion Properties

        #region Constructor

        public Lambda()
        { }

        #endregion Constructor

        #region Interface Methods

        public void Init(Guid agentId, TaskConfig config)
        {
            this.AgentId = agentId;
            this.Config = config;
        }

        public void Evaluate()
        {
            throw new NotImplementedException();
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }

        #endregion Interface Methods
    }
}