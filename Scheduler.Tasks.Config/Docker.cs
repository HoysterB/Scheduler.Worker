using System.Diagnostics;
using System.Text;

namespace Scheduler.Core
{
    public class Docker : IAgent
    {
        #region Properties

        public Guid AgentId { get; private set; }
        public TaskConfig Config { get; private set; }

        #endregion Properties

        #region Constructor

        public Docker()
        { }

        #endregion Constructor

        #region Methods

        private string DockerCommandBuilder()
        {
            StringBuilder command = new StringBuilder("docker.exe run ");
            command.Append(" --memory " + this.Config.Memory + "MB");
            command.Append(" --cpus " + this.Config.CpuCores);
            command.Append(" " + this.Config.ImageId);

            return command.ToString();
        }

        #endregion Methods

        #region Interface Methods

        public void Init(Guid agentId, TaskConfig taskConfig)
        {
            Console.WriteLine("Docker Init");
            this.AgentId = agentId;
            this.Config = taskConfig;
        }

        public void Evaluate()
        {
            Console.WriteLine("Docker Evaluate");
            return;

            Process process = new Process();
            string dockerCommand = this.DockerCommandBuilder();

            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = dockerCommand;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }

        #endregion Interface Methods
    }
}