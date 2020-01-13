using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbees.Engine
{
   public class EngineException: Exception
    {
        public EngineException(string message): base(message) { }
    }
}
