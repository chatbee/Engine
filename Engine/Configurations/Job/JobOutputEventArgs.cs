using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Job
{
    public class JobOutputEventArgs
    {
        public string Message { get; set; }
        public JobOutputEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
