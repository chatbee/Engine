﻿using Chatbees.Engine.Configurations.Tasks;
using Chatbees.Engine.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineTests.Helpers.Tasks
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class HappyTask : ITask
    {
        public string Name { get; set; }
        public Guid TaskId { get; set; } = Guid.NewGuid();
        public string TaskType { get; set; }
        public string Description { get; set; }
        public Exception TaskException { get; set; }
        public string NextTaskId { get; set; }
        public string ShouldSay { get; set; }
        public string ExecuteTask(JobContext context)
        {
            return this.ShouldSay;
        }
    }
}
