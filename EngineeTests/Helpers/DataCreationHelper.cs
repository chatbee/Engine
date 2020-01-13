using Chatbees.Engine.Configurations;
using Chatbees.Engine.Configurations.Tasks;
using EngineTests.Helpers.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineTests.Helpers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
        public static ThrowsTask MakeThrowsTask()
        {
            return new ThrowsTask
            {
                NextTaskId = null
            };
        }
        public static ErrorHandlerTask MakeErrorHandlerTask(ErrorAction action)
        {
            return new ErrorHandlerTask
            {
                Action = action
            };
        }
        public static HappyTask MakeHappyTask(string say)
        {
            return new HappyTask
            {
                ShouldSay = say
            };
        }
    }
}
