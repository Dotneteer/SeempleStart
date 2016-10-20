using System;
using FirebirdSql.Data.FirebirdClient;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// Event argument class with an FbCommand object
    /// </summary>
    public class FbCommandEventArgs : EventArgs
    {
        /// <summary>
        /// SqlCommand object
        /// </summary>
        public FbCommand FbCommand { get; private set; }

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="fbCommand">SqlCommand object</param>
        public FbCommandEventArgs(FbCommand fbCommand)
        {
            FbCommand = fbCommand;
        }
    }
}