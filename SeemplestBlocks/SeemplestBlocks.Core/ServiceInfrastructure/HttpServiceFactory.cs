using System;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.ServiceInfrastructure
{
    /// <summary>
    /// Ezt az osztályt használjuk arra, hogy létrehozzunk szolgáltatás objektumokat,
    /// amelyek kezelik a hívási kontextus elemeit.
    /// </summary>
    public static class HttpServiceFactory
    {
        private static IContextedServiceFactory s_FactoryInstance;

        /// <summary>
        /// A ServiceLocator objektumot adja vissza
        /// </summary>
        public static IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Az osztály statikus tagjait inicializálja
        /// </summary>
        static HttpServiceFactory()
        {
            Reset();
        }

        /// <summary>
        /// Beállítja a ServiceLocator objektumot az alapértelmezett értékre
        /// </summary>
        public static void Reset()
        {
            SetServiceLocator(ServiceManager.ServiceRegistry);
        }

        /// <summary>
        /// Beállítja a ServiceLocator objektumot a megadottra
        /// </summary>
        /// <param name="locator">ServiceLocator példány</param>
        public static void SetServiceLocator(IServiceLocator locator)
        {
            if (locator == null) throw new ArgumentNullException("locator");

            ServiceLocator = locator;

            // --- Az előző példány megszüntetése
            // ReSharper disable once SuspiciousTypeConversion.Global
            var disposable = s_FactoryInstance as IDisposable;
            if (disposable != null) disposable.Dispose();

            s_FactoryInstance = new BusinessContextServiceFactory(locator);
        }

        /// <summary>
        /// Létrehozza a megadott típusú szolgáltatás objektumot
        /// </summary>
        /// <param name="serviceType">A szolgáltatás objektum típusa</param>
        /// <returns>A szolgáltatás objektum példánya</returns>
        public static object CreateService(Type serviceType)
        {
            return s_FactoryInstance.CreateService(serviceType);
        }

        /// <summary>
        /// Létrehozza a megadott típusú szolgáltatás objektumot
        /// </summary>
        /// <typeparam name="TService">A szolgáltatás objektum típusa</typeparam>
        /// <returns>A szolgáltatás objektum példánya</returns>
        public static TService CreateService<TService>()
            where TService : class, IServiceObject
        {
            return s_FactoryInstance.CreateService<TService>();
        }
    }
}