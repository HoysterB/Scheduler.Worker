using Amazon.DynamoDBv2.DocumentModel;

namespace Scheduler
{
    public class Scheduler
    {
        private string TaskDefinitionTableName = "taskDefinition";
        private string TaskStateTableName = "taskState";

        public Scheduler()
        {
        }

        public void Run()
        {
            this.SubmitTask();
        }

        public List<TaskConfig> CheckTaskDefinitionExecutionTime(List<TaskConfig> tasksConfig)
        {
            List<TaskConfig> checkedTasksConfig = new List<TaskConfig>();
            foreach (var AgentConfig in tasksConfig)
                if (AgentConfig.StartTime <= DateTime.Now && DateTime.Now <= AgentConfig.EndTime)
                    checkedTasksConfig.Add(AgentConfig);

            return checkedTasksConfig;
        }

        private List<TaskConfig> GetCyclicTaskEnqueued()
        {
            // TODO Verificar se a tarefa já foi executada no intervalo de tempo determinado na configuração
            var dynamoDB = new DynamoDB(this.TaskDefinitionTableName);
            var taskDefinitionFilterList = new DynamoItemFilterList();
            taskDefinitionFilterList.Add(new DynamoItemFilter("taskState", ScanOperator.Equal, Enum.GetName(TaskState.Enqueued)));
            taskDefinitionFilterList.Add(new DynamoItemFilter("cyclic", ScanOperator.Equal, "true"));
            var tasksDefinition = dynamoDB.GetItemsByFilter(taskDefinitionFilterList);
            return tasksDefinition;
        }

        private List<TaskConfig> GetTasksEnqueued()
        {
            var dynamoDB = new DynamoDB(this.TaskDefinitionTableName);
            var taskDefinitionFilterList = new DynamoItemFilterList();
            taskDefinitionFilterList.Add(new DynamoItemFilter("taskState", ScanOperator.Equal, Enum.GetName(TaskState.Enqueued)));
            taskDefinitionFilterList.Add(new DynamoItemFilter("cyclic", ScanOperator.Equal, "false"));
            var tasksDefinition = dynamoDB.GetItemsByFilter(taskDefinitionFilterList);

            return tasksDefinition;
        }

        private List<IAgent> BuildAgentListToSubmition()
        {
            List<IAgent> agents = new List<IAgent>();
            List<TaskConfig> tasksConfig = new List<TaskConfig>();
            tasksConfig.AddRange(this.GetTasksEnqueued());
            tasksConfig.AddRange(this.GetCyclicTaskEnqueued());
            tasksConfig = this.CheckTaskDefinitionExecutionTime(tasksConfig);

            foreach (var taskConfig in tasksConfig)
            {
                var agent = StrategyMapping.ComponentAgentMapping[taskConfig.Component];
                agent.Init(Guid.NewGuid(), taskConfig);
                agents.Add(agent);

                //switch (taskConfig.Component)
                //{
                //    case Component.Docker:
                //        agents.Add(new Docker(Guid.NewGuid(), taskConfig));
                //        break;
                //    case Component.Lambda:
                //        break;
                //    case Component.Fargate:
                //        break;
                //    case Component.ECS:
                //        break;
                //    case Component.EC2:
                //        break;
                //}
            }

            return agents;
        }

        private void SubmitTask()
        {
            List<IAgent> agents = this.BuildAgentListToSubmition();
            foreach (var agent in agents)
            {
                //RegisterAction(); Registrar a acao de iniciação da execução

                agent.Evaluate();
                //Thread thread = new Thread(new ThreadStart(task.Evaluate));
                //thread.Start();
            }
        }
    }
}