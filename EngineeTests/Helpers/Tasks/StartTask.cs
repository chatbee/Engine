using Chatbees.Engine.Configurations.Tasks;
using Chatbees.Engine.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineTests.Helpers.Tasks
{
    public class StartTask : IStartTask
    {
        public StartTask()
        {
            this.TaskType = this.ToString();
        }
        public string Name { get; set; }
        public Guid TaskId { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public Exception TaskException { get; set; }
        public string NextTaskId { get; set; }
        public bool HasRun { get; set; }

        public string ExecuteTask(JobContext context)
        {
            this.HasRun = true;
            return "";
        }
    }
}
