using System;

namespace SeemplestBlocks.Core.ServiceInfrastructure.Exceptions
{
    /// <summary>
    /// Signs that there is no user in the service context
    /// </summary>
    public class NoUserInContextException : Exception
    {
        public NoUserInContextException() :
            base("No user information found in the current context, although it is expected.")
        {
        }
    }
}