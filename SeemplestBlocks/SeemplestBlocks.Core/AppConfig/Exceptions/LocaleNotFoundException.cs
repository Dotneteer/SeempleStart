using System;
using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.AppConfig.Exceptions
{
    /// <summary>
    /// Annak jelzése, hogy egy nyelvi környezetet nem találunk meg az adatbázisban.
    /// </summary>
    public class LocaleNotFoundException: BusinessOperationException
    {
        /// <summary>
        /// A nem talált nyelvi környezet kódja
        /// </summary>
        public string Code { get; private set; }

        public LocaleNotFoundException(string code) :
            base(String.Format("The specified locale cannot be found in the database: {0}", code))
        {
            Code = code;
        }
    }
}