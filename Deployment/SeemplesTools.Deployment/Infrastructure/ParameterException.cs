using System;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez a kivétel azt jelzi, hogy probléma volt valamelyik paraméterrel.
    /// </summary>
    public class ParameterException : ApplicationException
    {
        public ParameterException()
        {
        }

        public ParameterException(string message)
            : base(message)
        {
        }

        public ParameterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
