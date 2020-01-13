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
        public event EventHandler<JobOutputEventArgs> JobOutputEvent;
        public List<Type> RegisteredTypes { get; set; }
        public EngineService(MemoryCacheOptions cacheOptions)
        {
            this.CacheOptions = cacheOptions;
            this.Cache = new MemoryCache(this.CacheOptions);
            this.RegisteredTypes = new List<Type>();
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
            var itaskTypes = types.Where(t => t.GetInterface(nameof(ITask)) != null);
            if (itaskTypes.Count() != types.Length)
            {
                throw new ArgumentException("Error: Some of the types passed in do not implement ITask");
            }
            this.RegisteredTypes.AddRange(types);

            Types.RegisteredTypes.AddRange(types);
        }

    }
}
