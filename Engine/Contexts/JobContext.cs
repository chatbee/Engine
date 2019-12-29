using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Configurations.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations
{
    public class JobContext
    {
        public Guid InstanceId { get; set; }
        public JobConfiguration Configuration { get; set; }
        public ITask CurrentTask { get; set; }
        public List<ITask> Job { get; set; }
        public JobState ProcessJob(string input)
        {
            if (this.CurrentTask is null)
            {

            }
            return JobState.None;
        }
    }
}
