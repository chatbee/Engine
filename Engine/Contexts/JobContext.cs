using Chatbees.Engine.Configurations;
using Chatbees.Engine.Configurations.Job;
using Chatbees.Engine.Configurations.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chatbees.Engine.Contexts
{
    public class JobContext
    {
        private IErrorHandlingTask ErrorHandler { get; set; } 
        private ConfigContext ParentContext { get; set; }
        private JobExecutionMode ExecutionMode { get; set; }
        public event EventHandler<JobOutputEventArgs> OutputEvent;
        public Guid InstanceId { get; set; }
        public JobConfiguration Configuration { get; set; }
        public ITask CurrentTask { get; set; }
        public List<ITask> Tasks { get; set; }
        public JobContext(List<ITask> tasks, ConfigContext configContext, Guid instanceId, JobExecutionMode executionMode, JobConfiguration configuration)
        {
            this.Configuration = configuration;
            this.InstanceId = instanceId;
            this.Tasks = tasks;
            this.ParentContext = configContext;
            this.ExecutionMode = executionMode;
        }
        public JobState ProcessJob(string input)
        {
            this.ParentContext.WriteInstanceLog($"Processing job '{input}'");
            if (this.CurrentTask is null)
            {
                var errorHandler = Tasks.Where(t => t is IErrorHandlingTask).FirstOrDefault();

                if (errorHandler != null)
                {
                    this.ParentContext.WriteInstanceLog($"Assigning Error Handling Task '{errorHandler.TaskId}'");
                        this.ErrorHandler = errorHandler as IErrorHandlingTask;
                }

                this.ParentContext.WriteInstanceLog($"Current task is null, indicating this is a new job, starting at the start node");
                this.CurrentTask = Tasks.Where(t => t is IStartTask).FirstOrDefault() ?? throw new JobExecutionException("Error: No start task found");
            }

            while (this.CurrentTask != null)
            {
                string result = null;
                try
                {
                    if (this.CurrentTask is IErrorHandlingTask)
                    {
                        // Error handlers are noops
                    }
                    else
                    {
                        this.ParentContext.WriteInstanceLog($"Executing task '{this.CurrentTask.TaskId}'");
                        result = this.CurrentTask.ExecuteTask(this);
                    }

                }
                catch (Exception e)
                {
                    result = null;
                    this.ParentContext.WriteInstanceLog($"Task exception occured: {JsonConvert.SerializeObject(this.CurrentTask)} \nException:${e.ToString()}");

                    this.CurrentTask.TaskException = e;

                    if (this.ExecutionMode == JobExecutionMode.Debug)
                    {
                        OnOutput($"Sorry, I experienced an error during job execution: {e.Message}");
                    }

                    if (this.ErrorHandler is null)
                    {
                        OnOutput("Unfortunately, I encountered an error processing your request, but no one told me what to do! Can you believe that? Let's try doing something different.");
                    }
                    else if (this.ErrorHandler.Action == ErrorAction.StopJob)
                    {
                        OnOutput("Let's try something else, I seem to have encountered an error");
                        return JobState.Finished;
                    }
                    else if (this.ErrorHandler.Action == ErrorAction.ContinueJob)
                    {
                        //Nothing to do, noop
                    }

                }

                if (!string.IsNullOrEmpty(result))
                {
                    this.ParentContext.WriteInstanceLog("Sending result to event subscriber");
                    OnOutput(result);

                }

                var state = this.DetermineJobState();
                if (state == JobState.Finished)
                {
                    return state;
                }
            }
            return JobState.Finished;
        }
        /// <summary>
        /// Determines a job state based on whether it is capable for more progression or not.
        /// </summary>
        /// <returns>A JobState which can be returned back to the caller to let it know the status of the current job</returns>
        private JobState DetermineJobState()
        {
            //check next node
            if (string.IsNullOrEmpty(this.CurrentTask.NextTaskId))
            {
                //next node is null or empty so there is no further to go
                ParentContext.WriteInstanceLog("Conversation Requires No More Nodes.");
                this.CurrentTask = null;
                return JobState.Finished;
            }
            else
            {

                string nextTaskId = this.CurrentTask.NextTaskId;
                ParentContext.WriteInstanceLog("Conversation Requires Next Node ID '" + nextTaskId + "'");

                //next node is available and has been set
                var nextTask = this.Tasks.Where(f => f.TaskId == Guid.Parse(nextTaskId)).FirstOrDefault();

                if (nextTask == null)
                {
                    this.ParentContext.WriteInstanceLog("Unable to progress conversation as task ID '" + nextTaskId + "' cannot be found.");
                    throw new JobExecutionException("Unable to progress conversation as task ID '" + nextTaskId + "' cannot be found.");
                }
                else
                {
                    ParentContext.WriteInstanceLog("Progressed to task '" + nextTask.TaskId + "'");
                }

                this.CurrentTask = nextTask;

                return JobState.Progressing;
            }
        }
        
        private void OnOutput(string output)
        {
            var args = new JobOutputEventArgs(this.Configuration.Id,output);

            this.OutputEvent?.Invoke(this, args);
        }
    }
}
