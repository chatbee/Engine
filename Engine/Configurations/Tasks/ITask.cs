using Chatbees.Engine.Contexts;
using Chatbees.Engine.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Tasks
{
    /// <summary>
    /// Represents a task node which defines an engine process flow.
    /// </summary>
    [JsonConverter(converterType: typeof(JsonCreationConverter))]
    public interface ITask
    {

        public string Name { get; set; }
        Guid TaskId { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public Exception TaskException { get; set; }
        /// <summary>
        /// This is the 'linked-list' esque pointer to the next task
        /// </summary>
        public string NextTaskId { get; set; }

        private class JsonCreationConverter : JsonCreationConverter<ITask>
        {
            protected override ITask Create(Type objectType,
              Newtonsoft.Json.Linq.JObject jObject)
            {
                string taskType;
                if (jObject.Value<string>("taskType") != null)
                {
                    taskType = jObject.Value<string>("taskType");
                }
                else if (jObject.Value<string>("TaskType") != null)
                {
                    taskType = jObject.Value<string>("TaskType");
                }
                else
                {
                    throw new Newtonsoft.Json.JsonException("Error finding Task Type during serialization");
                }
                var type = Type.GetType(taskType);
                return (ITask)Activator.CreateInstance(type);
            }
        }

        /// <summary>
        /// Executes a task within the given context.
        /// </summary>
        /// <remarks>
        /// Should throw a <see cref="TaskExecutionException "/> if an exception occurs
        /// </remarks>
        /// <param name="context">The current context</param>
        /// <returns>Should only return a value if its an output from the task and is not modifying the context</returns>
        /// <exception cref="TaskExecutionException"
        public abstract string ExecuteTask(JobContext context);
    }
}
