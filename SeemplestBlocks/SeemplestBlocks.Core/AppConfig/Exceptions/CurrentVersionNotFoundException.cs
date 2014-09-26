using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.AppConfig.Exceptions
{
    /// <summary>
    /// Annak jelzése, hogy egy nyelvi környezetet nem találunk meg az adatbázisban.
    /// </summary>
    public class CurrentVersionNotFoundException: BusinessOperationException
    {
        public CurrentVersionNotFoundException() :
            base("There's no information stored about the current configuration version")
        {
        }
    }
}