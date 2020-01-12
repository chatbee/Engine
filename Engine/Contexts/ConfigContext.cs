using Chatbees.Engine.Configurations;
using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Configurations.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Contexts
{
    public class ConfigContext
    {
        private string IntialInput { get; set; }
        public JobContext CurrentJob { get; set; }
        public Guid InstanceId { get; set; } = Guid.NewGuid();
        private List<string> InstanceLogs { get; set; } = new List<string>();
        private JobConfiguration Config { get; set; }
        private JobExecutionMode Mode { get; set; }
        public ConfigContext(JobConfiguration jobConfiguration, JobExecutionMode executionMode)
        {
            this.Config = jobConfiguration;
            this.Mode = executionMode;
        }
        public JobContext CreateJobContext(string input)
        {
            this.IntialInput = input;

            var clonedJobs = JsonConvert.DeserializeObject<List<ITask>>(JsonConvert.SerializeObject(this.Config.Tasks, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            var job = new JobContext(clonedJobs, this, this.InstanceId, this.Mode);
            


            return job;

        }
        public void WriteInstanceLog(string log)
        {
            this.InstanceLogs.Add($"${DateTime.UtcNow} - ${log}");
        }
    }
}
