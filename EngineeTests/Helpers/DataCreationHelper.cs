using Chatbees.Engine.Configurations;
using EngineTests.Helpers.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineTests.Helpers
{
    public static class DataCreationHelper
    {
        public static JobConfiguration MakeJobConfiguration()
        {
            return new JobConfiguration
            {
                Name = "test",
                Description = "testing",
                Id = Guid.NewGuid(),
            };
        }
        public static StartTask MakeStartTask()
        {
            return new StartTask
            {
                HasRun = false,
                NextTaskId = null,
            };
        }
    }
}
