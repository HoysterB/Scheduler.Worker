using Amazon.DynamoDBv2.DocumentModel;

namespace Scheduler.Core
{
    public class TaskConfig
    {
        #region Properties

        public Guid TaskId { get; set; }
        public int Memory { get; set; }
        public int Priority { get; set; }
        public int Timeout { get; set; }
        public int CpuCores { get; set; }
        public string ImageId { get; set; }
        public string Credentials { get; set; }
        public int Period { get; set; }
        public bool Cyclic { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreationTime { get; set; }
        public Component Component { get; set; }
        public TaskGroup TaskGroup { get; set; }
        public TaskState TaskState { get; set; }
        public TaskType TaskType { get; set; }
        public Platform Platform { get; set; }

        #endregion Properties

        #region Contructor

        public TaskConfig GetFromDynamoDB(Document agentConfig)
        {
            var taskConfig = new TaskConfig();

            taskConfig.TaskId = agentConfig["taskId"].AsGuid();
            taskConfig.ImageId = agentConfig["imageId"].AsString();
            taskConfig.Component = (Component)Enum.Parse(typeof(Component), agentConfig["component"].AsString());
            taskConfig.CreationTime = Convert.ToDateTime(agentConfig["creatinTime"].AsString());
            taskConfig.TaskGroup = (TaskGroup)Enum.Parse(typeof(TaskGroup), agentConfig["taskGroup"].AsString());
            taskConfig.Priority = agentConfig["priority"].AsInt();
            taskConfig.TaskState = (TaskState)Enum.Parse(typeof(TaskState), agentConfig["taskState"].AsString());
            taskConfig.Timeout = agentConfig["timeout"].AsInt();
            taskConfig.Credentials = agentConfig["credentials"].AsString();
            taskConfig.TaskType = (TaskType)Enum.Parse(typeof(TaskType), agentConfig["taskType"].AsString());
            taskConfig.CpuCores = agentConfig["cpuCores"].AsInt();
            taskConfig.StartTime = agentConfig["startTime"].AsString() == string.Empty ? Convert.ToDateTime("00:00:00") : Convert.ToDateTime(agentConfig["startTime"].AsString());
            taskConfig.EndTime = agentConfig["endTime"].AsString() == string.Empty ? Convert.ToDateTime("23:59:00") : Convert.ToDateTime(agentConfig["endTime"].AsString());
            taskConfig.Period = agentConfig["period"].AsInt();
            taskConfig.Platform = (Platform)Enum.Parse(typeof(Platform), agentConfig["platform"].AsString());
            taskConfig.Cyclic = agentConfig["cyclic"].AsBoolean();
            taskConfig.Memory = agentConfig["memory"].AsInt();

            return taskConfig;
        }

        #endregion Contructor
    }
}