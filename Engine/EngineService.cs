using Chatbees.Engine.Configurations;
using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Configurations.Tasks;
using Chatbees.Engine.Contexts;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chatbees.Engine
{
    public class EngineService
    {
        private MemoryCache Cache { get; set; }
        private MemoryCacheOptions CacheOptions { get; set; }
        private bool HasRegistered { get; set; }
        public event EventHandler<JobOutputEventArgs> JobOutputEvent;
        public EngineService(MemoryCacheOptions cacheOptions)
        {
            this.CacheOptions = cacheOptions;
            this.Cache = new MemoryCache(this.CacheOptions);
        }

        public Guid NewInstance(JobConfiguration jobConfiguration, JobExecutionMode executionMode, MemoryCacheEntryOptions entryOptions)
        {
            if (!this.HasRegistered)
            {
                throw new EngineException("Error: You must register types before invoking the engine");
            }
            ConfigContext configContext;

            configContext = new ConfigContext(jobConfiguration, executionMode);

            this.Cache.Set(configContext.InstanceId, configContext, entryOptions);

            return configContext.InstanceId;
        }

        public void ProcessInput(string input, Guid instanceId)
        {
            if (!this.HasRegistered)
            {
                throw new EngineException("Error: You must register types before invoking the engine");
            }
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
            jobContext.OutputEvent += JobContext_OutputEvent;

            var jobState = jobContext.ProcessJob(input);
            if (jobState == JobState.Finished)
            {
                cachedContext.CurrentJob = null;
            }

            this.Cache.Set(instanceId, cachedContext);
        }

        private void JobContext_OutputEvent(object sender, JobOutputEventArgs e)
        {
            this.JobOutputEvent?.Invoke(sender, e);
        }

        public void RegisterTypes(params Type[] types)
        {
            if (types.Length == 0)
            {
                throw new ArgumentException("You can't register 0 types");
            }
            var itaskTypes = types.Where(t => t.GetInterface(nameof(ITask)) != null);
            if (itaskTypes.Count() != types.Length)
            {
                throw new ArgumentException("Error: Some of the types passed in do not implement ITask");
            }
            Types.RegisteredTypes.AddRange(types);
            this.HasRegistered = true;
        }

    }
}
