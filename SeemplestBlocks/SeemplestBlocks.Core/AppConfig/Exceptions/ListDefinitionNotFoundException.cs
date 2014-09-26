using System;
using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.AppConfig.Exceptions
{
    /// <summary>
    /// Annak jelzése, hogy egy lista definícióját nem találunk meg az adatbázisban.
    /// </summary>
    public class ListDefinitionNotFoundException: BusinessOperationException
    {
        /// <summary>
        /// A nem talált lista azonosítója
        /// </summary>
        public string ListId { get; private set; }

        public ListDefinitionNotFoundException(string listId) :
            base(String.Format("The specified list definition cannot be found in the database: {0}", listId))
        {
            ListId = listId;
        }
    }
}