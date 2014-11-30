using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace TISAppenderLog4net
{
    public class Log4NetException : System.Exception
    {
        public Log4NetException() : base() { TISMessage = string.Empty; m_StackTrace = string.Empty; }
        public Log4NetException(string message) : base(message) { TISMessage = message; m_StackTrace = string.Empty; }
        public Log4NetException(string message, System.Exception inner)
            : base(inner.Message, inner)
        {
            TISMessage = message;
            this.m_StackTrace = inner.StackTrace;
        }

        public Log4NetException(string message, System.Exception inner, StackFrame frame)
            : base(inner.Message, inner)
        {
            TISMessage = message;
            m_StackTrace = inner.StackTrace;
            MethodBase method = frame.GetMethod();
            Source = method.Module.Name;
            m_Class = method.DeclaringType.ToString();
            m_Method = method.Name;
        }

        public string TISMessage;
        public string m_Class;
        public string m_Method;
        public string m_StackTrace;

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected Log4NetException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
