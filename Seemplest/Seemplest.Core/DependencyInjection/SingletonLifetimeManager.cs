using Seemplest.Core.Common;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This lifetime manager creates a new instance of the managed type for each 
    /// <see cref="GetObject"/> call.
    /// </summary>
    public class SingletonLifetimeManager : LifetimeManagerBase
    {
        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public SingletonLifetimeManager()
        {
        }

        /// <summary>
        /// Creates an instance of this lifetime manager using the specified singleton object.
        /// </summary>
        /// <param name="instance">Object instance to retrieve</param>
        public SingletonLifetimeManager(object instance)
        {
            ServiceObjectType = instance.GetType();
            Instance = instance;
        }

        /// <summary>
        /// Stores the singleton object used by this lifetime manager
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// Retrieve an object from the backing store associated with this Lifetime manager.
        /// </summary>
        /// <returns>
        /// The object retrieved by the lifetime manager.
        /// </returns>
        public override object GetObject()
        {
            return Instance ?? (Instance = CreateObjectInstance());
        }

        /// <summary>
        /// Resets the state of the lifetime manager
        /// </summary>
        public override void ResetState()
        {
            Instance = null;
        }
    }
}