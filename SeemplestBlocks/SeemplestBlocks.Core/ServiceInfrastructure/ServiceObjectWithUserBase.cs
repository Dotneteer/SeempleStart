using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Core.ServiceInfrastructure.Exceptions;

namespace SeemplestBlocks.Core.ServiceInfrastructure
{
    /// <summary>
    /// Ez az osztály azoknak a szolgáltatásoknak az ősosztálya, amelyek hozzáférnek
    /// a felhasználó azonosítójához
    /// </summary>
    public class ServiceObjectWithUserBase : ServiceObjectBase
    {
        public string ServicedUserId
        {
            get
            {
                var userContext = CallContext.Get<UserIdContextItem>();
                if (userContext == null)
                {
                    throw new NoUserInContextException();
                }
                return userContext.Value;
            }
        }
    }
}