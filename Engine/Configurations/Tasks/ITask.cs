using Chatbees.Engine.Contexts;
using Chatbees.Engine.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
                else if (jObject.Value<string>("$type") != null)
                {
                    taskType = jObject.Value<string>("$type").Split(',')[0];
                }
                else if (jObject.Value<string>("$Type") != null)
                {
                    taskType = jObject.Value<string>("$Type").Split(',')[0];
                }
                else
                {
                    throw new Newtonsoft.Json.JsonException("Error finding Task Type during serialization");
                }

                var customType = Engine.Types.RegisteredTypes.Where(f => f.FullName == taskType).FirstOrDefault();

                if (customType != null)
                {
                    return (ITask)Activator.CreateInstance(customType);
                }
                else
                {
                    throw new EngineException($"Error requested Type {customType} was not registered. Type should be registered before invoking automation.");
                }

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
