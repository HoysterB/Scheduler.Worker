namespace Scheduler
{
    public static class StrategyMapping
    {
        public static Dictionary<Component, IAgent> ComponentAgentMapping = new Dictionary<Component, IAgent>()
            {
                { Component.Docker, new Docker() },
                { Component.Lambda, new Lambda() },
                { Component.ECS, new ECS() }
            };
    }

    public enum TaskType
    {
        DataSync,
        Notification
    }

    public enum TaskGroup
    {
        Information,
        Replication
    }

    public enum Platform
    {
        AWS,
        GCP,
        Azure,
        Local
    }

    public enum Component
    {
        ECS,
        Lambda,
        Docker,
        EC2,
        Fargate
    }

    public enum TaskState
    {
        Enqueued,
        Scheduled,
        Processing,
        Finished
    }

    public enum AgentState
    {
        Scheduled,
        Processing,
        Aborted,
        Finished
    }
}