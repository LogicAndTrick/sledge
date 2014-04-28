using System;

namespace Sledge.Common.Mediator
{
    public class MediatorExceptionEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public MediatorExceptionEventArgs(string message, Exception exception)
        {
            Exception = exception;
            Message = message;
        }
    }
}