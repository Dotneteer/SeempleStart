using System.Diagnostics.CodeAnalysis;

namespace Seemplest.Core.DependencyInjection
{
    /// <summary>
    /// This class implements a service registry that has a part set in code and another that
    /// can override services set in code.
    /// </summary>
    public abstract class OverridableServiceRegistryBase : ServiceRegistry
    {
        private const string PRESET = "Preset";
        private const string CONFIGURABLE = "Configurable";

        private readonly ServiceContainer _preset;
        private readonly ServiceContainer _configurable;

        protected OverridableServiceRegistryBase()
        {
            _preset = new ServiceContainer(PRESET);
            _configurable = new ServiceContainer(CONFIGURABLE, _preset);
            DefaultContainer = _configurable;
            Containers.Add(CONFIGURABLE, DefaultContainer);
            Containers.Add(PRESET, _preset);
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            SetDefaultServices(_preset);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        /// Gets the configurable container of this instance.
        /// </summary>
        /// <returns>The configurable container of this instance.</returns>
        public ServiceContainer GetConfigurableContainer()
        {
            return _configurable;
        }

        /// <summary>
        /// Gets the preset container of this instance.
        /// </summary>
        /// <returns>The preset container of this instance</returns>
        public ServiceContainer GetPresetContainer()
        {
            return _preset;
        }

        /// <summary>
        /// Override this method to sets the default services
        /// </summary>
        /// <param name="container"></param>
        [ExcludeFromCodeCoverage]
        protected virtual void SetDefaultServices(ServiceContainer container)
        {
        }
    }
}