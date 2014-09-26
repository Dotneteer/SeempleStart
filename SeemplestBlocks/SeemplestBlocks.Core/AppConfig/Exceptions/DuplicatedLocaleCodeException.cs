using System;
using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.AppConfig.Exceptions
{
    /// <summary>
    /// Annak jelzése, hogy egy nyelvi környezet létrehozásakor nem egyedi kódot használunk.
    /// </summary>
    public class DuplicatedLocaleCodeException: BusinessOperationException
    {
        public DuplicatedLocaleCodeException(string code) :
            base(String.Format("Locale code must be unique. The following code is duplicated: {0}", code))
        {
        }
    }
}