using Chatbees.Engine.Configurations.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Tasks
{
    public static class ExtensionMethods
    {
        public static void LinkTo(this ITask task, ITask nextTask)
        {
            task.NextTaskId = nextTask.TaskId.ToString();
        }
    }
}
