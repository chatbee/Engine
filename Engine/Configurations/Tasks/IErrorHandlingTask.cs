using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Tasks
{
    public interface IErrorHandlingTask : ITask
    {
        public ErrorAction Action { get; set; }
    }
    public enum ErrorAction 
    {
        StopJob,
        ContinueJob,
    }
}
