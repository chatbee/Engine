using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Job
{
    public class JobOutputEventArgs
    {
        public Guid JobConfigurationId { get; set; }
        public string Message { get; set; }
        public JobOutputEventArgs(Guid jobConfigurationId, string message)
        {
            this.JobConfigurationId = JobConfigurationId;
            this.Message = message;
        }
    }
}
