using System;

namespace CliParser
{
    /// <summary>
    /// Exception that is thrown when an internal error occurs whil parsing
    /// command line options.
    /// </summary>
    [Serializable]
    public class CliInternalException : Exception
    {
        public CliInternalException() { }
        public CliInternalException(string message) : base(message) { }
        public CliInternalException(string message, Exception inner) : base(message, inner) { }
        protected CliInternalException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    /// <summary>
    /// Exception that is thrown when the given command line options are 
    /// incompatible with the requirements.
    /// </summary>
    [Serializable]
    public class CliOptionsException : Exception
    {
        public CliOptionsException() { }
        public CliOptionsException(string message) : base(message) { }
        public CliOptionsException(string message, Exception inner) : base(message, inner) { }
        protected CliOptionsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}