using System;
using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.AppConfig.Exceptions
{
    /// <summary>
    /// Annak jelzése, hogy egy nyelvi környezet létrehozásakor nem egyedi megnevezést használunk.
    /// </summary>
    public class DuplicatedLocaleDisplayNameException: BusinessOperationException
    {
        public DuplicatedLocaleDisplayNameException(string name) :
            base(String.Format("Locale name must be unique. The following name is duplicated: {0}", name))
        {
        }
    }
}