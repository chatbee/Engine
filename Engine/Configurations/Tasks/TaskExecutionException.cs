using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Tasks
{
   public class TaskExecutionException: Exception
    {
        /// <summary>
        /// Method for handling engine exceptions and logging.
        /// </summary>
        /// <param name="message">The error message</param>
        public TaskExecutionException(string message): base(message)
        {

        }
    }
}
