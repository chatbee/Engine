using Chatbees.Engine;
using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Configurations.Tasks;
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
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
        public void EngineServiceThrowsWithoutRegistering()
        {
            var jobConfig = DataCreationHelper.MakeJobConfiguration();
            Assert.Throws<EngineException>(delegate
            {

                var instanceId = service.NewInstance(jobConfig, Chatbees.Engine.Configurations.Job.JobExecutionMode.Debug, entryOptions);
            });

        }
        [Fact]
        public void EngineServiceThrowsWithZeroRegistered()
        {
            var jobConfig = DataCreationHelper.MakeJobConfiguration();
            Assert.Throws<ArgumentException>(delegate
            {

                service.RegisterTypes();
            });

        }
        [Fact]
        public void EngineServiceThrowsWithNonITaskRegistered()
        {
            var jobConfig = DataCreationHelper.MakeJobConfiguration();
            Assert.Throws<ArgumentException>(delegate
            {

                service.RegisterTypes(typeof(string));
            });

        }
        [Fact]
        public void EngineServiceThrowsWithZeroRegisteredProcessInput()
        {
            Assert.Throws<EngineException>(delegate
            {
                service.ProcessInput("hi", Guid.Empty);
               
            });

        } 
        [Fact]
        public void EngineServiceThrowsThereIsNothingInTheCacheByThatID()
        {
            Assert.Throws<EngineException>(delegate
            {
                service.ProcessInput("hi", Guid.Empty);
               
            });

        }

        [Fact]
        public void EngineServiceThrowsWithoutAStartTask()
        {
            var jobConfig = DataCreationHelper.MakeJobConfiguration();

            service.RegisterTypes(typeof(StartTask));
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
        [Fact]
        public void EngineServiceHandlesErrorsWithoutErrorHandlingTask()
        {
            var jc = DataCreationHelper.MakeJobConfiguration();
            var st = DataCreationHelper.MakeStartTask();
            var tt = DataCreationHelper.MakeThrowsTask();
            st.LinkTo(tt);

            jc.Tasks.Add(st);
            jc.Tasks.Add(tt);

            service.RegisterTypes(st.GetType(), tt.GetType());

            var instanceId = service.NewInstance(jc, Chatbees.Engine.Configurations.Job.JobExecutionMode.Debug, entryOptions);
            var num = 0;
            service.JobOutputEvent += delegate (object sender, JobOutputEventArgs e)
            {
                Assert.NotEqual(Guid.Empty, e.JobConfigurationId);
                if (num == 0)
                {
                    Assert.Equal("Start task ran", e.Message);
                }
                else if (num == 1)
                {
                    Assert.Equal("Sorry, I experienced an error during job execution: The method or operation is not implemented.", e.Message);
                }
                else if (num == 2)
                {
                    Assert.Equal("Unfortunately, I encountered an error processing your request, but no one told me what to do! Can you believe that? Let's try doing something different.", e.Message);
                }
                else
                {
                    Assert.False(true, "Num got larger than expected!");
                }
                num++;
            };

            service.ProcessInput("Hi!", instanceId);
        }
        [Fact]
        public void EngineServiceHandlesErrorsWithErrorHandlerIsStop()
        {
            var jc = DataCreationHelper.MakeJobConfiguration();

            var st = DataCreationHelper.MakeStartTask();
            var eth = DataCreationHelper.MakeErrorHandlerTask(ErrorAction.StopJob);
            var tt = DataCreationHelper.MakeThrowsTask();
            var ht = DataCreationHelper.MakeHappyTask("Hello world");
            st.LinkTo(eth);
            eth.LinkTo(tt);
            tt.LinkTo(ht);

            jc.Tasks.Add(st);
            jc.Tasks.Add(tt);
            jc.Tasks.Add(eth);
            jc.Tasks.Add(ht);


            service.RegisterTypes(st.GetType(), tt.GetType(), ht.GetType(), eth.GetType());

            var instanceId = service.NewInstance(jc, Chatbees.Engine.Configurations.Job.JobExecutionMode.Debug, entryOptions);
            var num = 0;
            service.JobOutputEvent += delegate (object sender, JobOutputEventArgs e)
            {
                Assert.NotEqual("Hello world", e.Message);
                Assert.NotEqual(Guid.Empty, e.JobConfigurationId);
                if (num == 0)
                {
                    Assert.Equal("Start task ran", e.Message);
                }
                else if (num == 1)
                {
                    Assert.Equal("Sorry, I experienced an error during job execution: The method or operation is not implemented.", e.Message);
                }
                else if (num == 2)
                {
                    Assert.Equal("Let's try something else, I seem to have encountered an error", e.Message);
                }
                else
                {
                    Assert.False(true, "Num got larger than expected!");
                }
                num++;
            };

            service.ProcessInput("Hi!", instanceId);
        }
        [Fact]
        public void EngineServiceHandlesErrorsWithErrorHandlerContinue()
        {
            var jc = DataCreationHelper.MakeJobConfiguration();

            var st = DataCreationHelper.MakeStartTask();
            var eth = DataCreationHelper.MakeErrorHandlerTask(ErrorAction.ContinueJob);
            var tt = DataCreationHelper.MakeThrowsTask();
            var ht = DataCreationHelper.MakeHappyTask("Hello world");
            st.LinkTo(eth);
            eth.LinkTo(tt);
            tt.LinkTo(ht);

            jc.Tasks.Add(st);
            jc.Tasks.Add(tt);
            jc.Tasks.Add(eth);
            jc.Tasks.Add(ht);


            service.RegisterTypes(st.GetType(), tt.GetType(), ht.GetType(), eth.GetType());

            var instanceId = service.NewInstance(jc, Chatbees.Engine.Configurations.Job.JobExecutionMode.Debug, entryOptions);
            var num = 0;
            service.JobOutputEvent += delegate (object sender, JobOutputEventArgs e)
            {
                Assert.NotEqual(Guid.Empty, e.JobConfigurationId);
                if (num == 0)
                {
                    Assert.Equal("Start task ran", e.Message);
                }
                else if (num == 1)
                {
                    Assert.Equal("Sorry, I experienced an error during job execution: The method or operation is not implemented.", e.Message);
                }
                else if (num == 2)
                {
                    Assert.Equal("Hello world", e.Message);
                }
                else
                {
                    Assert.False(true, "Num got larger than expected!");
                }
                num++;
            };

            service.ProcessInput("Hi!", instanceId);
        }
    }
}
