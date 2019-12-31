using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Job
{
    public class JobExecutionException: Exception
    {
        public JobExecutionException(string message) : base(message) { }
    }
}
