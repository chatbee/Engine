using Chatbees.Engine.Configurations;
using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Contexts;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine
{
    public class EngineService
    {
        private MemoryCache Cache { get; set; }
        private MemoryCacheOptions CacheOptions { get; set; }
    
        public EngineService(MemoryCacheOptions cacheOptions)
        {
            this.CacheOptions = cacheOptions;
            this.Cache = new MemoryCache(this.CacheOptions);
        }

        public Guid NewInstance(JobConfiguration jobConfiguration, JobExecutionMode executionMode, MemoryCacheEntryOptions entryOptions)
        {
            ConfigContext configContext;

            configContext = new ConfigContext(jobConfiguration, executionMode);

            this.Cache.Set(configContext.InstanceId, configContext, entryOptions);

            return configContext.InstanceId;
        }

        public void ProcessInput(string input, Guid instanceId)
        {
            this.Cache.TryGetValue(instanceId, out ConfigContext cachedContext);

            JobContext jobContext;
            if (cachedContext is null)
            {
                throw new EngineException("Error, there is no cachedContext under that id");
            }

            if (cachedContext.CurrentJob is null)
            {
                jobContext = cachedContext.CreateJobContext(input);
            }
            else
            {
                jobContext = cachedContext.CurrentJob;
            }

            var jobState = jobContext.ProcessJob(input);
            if (jobState == JobState.Finished)
            {
                cachedContext.CurrentJob = null;
            }

            this.Cache.Set(instanceId, cachedContext);
        }
    }
}
