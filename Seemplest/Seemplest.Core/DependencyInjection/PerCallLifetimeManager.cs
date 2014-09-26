namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This lifetime manager creates a new instance of the managed type for each 
    /// <see cref="GetObject"/> call.
    /// </summary>
    public class PerCallLifetimeManager : LifetimeManagerBase
    {
        /// <summary>
        /// Retrieve an object from the backing store associated with this Lifetime manager.
        /// </summary>
        /// <returns>
        /// The object retrieved by the lifetime manager.
        /// </returns>
        public override object GetObject()
        {
            return CreateObjectInstance();
        }

        /// <summary>
        /// Resets the state of the lifetime manager
        /// </summary>
        public override void ResetState()
        {
            // --- Nothing to do
        }
    }
}