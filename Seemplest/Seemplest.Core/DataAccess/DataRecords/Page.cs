using System.Collections.Generic;

namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This class represents a result from a paged request
    /// </summary>
    /// <typeparam name="T">Typy of the poco</typeparam>
    public class Page<T>
    {
        /// <summary>
        /// Gets or sets the cardinal number of the current page.
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of total pages.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of total items.
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets the list of items.
        /// </summary>
        public List<T> Items { get; set; }
    }
}