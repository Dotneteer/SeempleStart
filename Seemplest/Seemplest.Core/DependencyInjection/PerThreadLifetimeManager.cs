using System;
using Seemplest.Core.Common;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This lifetime manager creates a new instance of the managed type for each 
    /// <see cref="GetObject"/> call.
    /// </summary>
    public class PerThreadLifetimeManager : LifetimeManagerBase
    {
        [ThreadStatic]
        private static object s_ThreadInstance;
        
        /// <summary>
        /// Stores the singleton object used by this lifetime manager
        /// </summary>
        public object Instance
        {
            get { return s_ThreadInstance; }
        }

        /// <summary>
        /// Retrieve an object from the backing store associated with this Lifetime manager.
        /// </summary>
        /// <returns>
        /// The object retrieved by the lifetime manager.
        /// </returns>
        public override object GetObject()
        {
            return s_ThreadInstance ?? (s_ThreadInstance = CreateObjectInstance());
        }

        /// <summary>
        /// Resets the state of the lifetime manager
        /// </summary>
        public override void ResetState()
        {
            s_ThreadInstance = null;
        }
    }
}