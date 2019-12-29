using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine.Configurations.Job
{
    /// <summary>
    /// Indicates the current state of the conversation
    /// </summary>
    public enum JobState
    {
        // <summary>
        /// Indicates conversation has not started.
        /// </summary>
        None,
        // <summary>
        /// Indicates conversation is progressing.
        /// </summary>
        Progressing,
        // <summary>
        /// Indicates conversation is awaiting a response from the user.
        /// </summary>
        AwaitingResponse,
        // <summary>
        /// Indicates conversation has finished.
        /// </summary>
        Finished
    }
}


