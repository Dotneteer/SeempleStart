namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This interface defines the behavior of a service registry that has a collection of containers
    /// and behaves like a service locator through a default container
    /// </summary>
    public interface IServiceRegistry: IServiceContainerCollection, IServiceLocator
    {
        /// <summary>
        /// Gets the default container of the service registry
        /// </summary>
        IServiceContainer DefaultContainer { get; }
    }
}