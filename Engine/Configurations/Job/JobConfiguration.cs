using Chatbees.Engine.Configurations.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class JobConfiguration
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ITask> Tasks { get; set; } = new List<ITask>();
    }
}
