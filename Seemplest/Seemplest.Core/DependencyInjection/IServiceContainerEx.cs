using System;
using System.Collections.Generic;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class defines operations to discover circular references while resolving a
    /// service type.
    /// </summary>
    internal interface IServiceContainerEx : IServiceContainer
    {
        /// <summary>
        /// Gets the service object from the container using the collection of types 
        /// already visited during resolution.
        /// </summary>
        /// <param name="serviceType">Type of the service</param>
        /// <param name="visitedTypes">Types visited during resolution</param>
        /// <returns></returns>
        object GetService(Type serviceType, List<Type> visitedTypes);
    }
}