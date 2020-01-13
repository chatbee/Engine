using Chatbees.Engine;
using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Contexts;
using EngineTests.Helpers;
using EngineTests.Helpers.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EngineTests
{
    public class EngineServiceTests
    {
        private readonly EngineService service;
        private readonly MemoryCacheEntryOptions entryOptions;
        public EngineServiceTests()
        {
            var o = new MemoryCacheOptions
            {
                ExpirationScanFrequency = new TimeSpan(0, 10, 0)
            };
            this.entryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            this.service = new EngineService(o);
        }
        
        [Fact]
        public void EngineServiceThrowsWithoutAStartTask()
        {
           

            var jobConfig = DataCreationHelper.MakeJobConfiguration();

            var instanceId = service.NewInstance(jobConfig, Chatbees.Engine.Configurations.Job.JobExecutionMode.Debug, entryOptions);
            Assert.Throws<JobExecutionException>(delegate
            {
                service.ProcessInput("hi!", instanceId);
            });
        }
        [Fact]
        public void EngineServiceSetsAndExecutesStartTaskProperly()
        {
            var jobConfig = DataCreationHelper.MakeJobConfiguration();

            jobConfig.Tasks.Add(DataCreationHelper.MakeStartTask());


            service.RegisterTypes(typeof(StartTask));
            

            var instanceId = service.NewInstance(jobConfig, Chatbees.Engine.Configurations.Job.JobExecutionMode.Debug, entryOptions);

            service.JobOutputEvent += delegate (object sender, JobOutputEventArgs e)
            {
                Assert.IsType<JobContext>(sender);
                Assert.Equal("Start task ran", e.Message);
            };

            service.ProcessInput("Hi!", instanceId);
        }
    }
}
