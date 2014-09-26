using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration.ResourceConnections;
using Seemplest.Core.TypeResolution;

namespace Seemplest.Core.Queue.Configuration
{
    [DisplayName("QueueConnection")]
    public class QueueConnectionProvider : ResourceConnectionProviderBase
    {
        private const string TYPE = "type";
        private const string INITIAL_PARAMS = "initialParams";

        /// <summary>
        /// Gets the type implementing the queue
        /// </summary>
        public Type QueueType { get; private set; }

        /// <summary>
        /// Gets the string describing the initial parameters of the queue provider
        /// </summary>
        public string InitialParams { get; private set; }

        /// <summary>
        /// Creates a new instance of this provider
        /// </summary>
        /// <param name="queueType">Type of the queue</param>
        /// <param name="initialParams">Initial provider parameters</param>
        public QueueConnectionProvider(Type queueType, string initialParams)
        {
            QueueType = queueType;
            InitialParams = initialParams;
        }

        /// <summary>
        /// Initializes the provider using the specified XML element
        /// </summary>
        /// <param name="element"></param>
        public QueueConnectionProvider(XElement element)
        {
            ReadFromXml(element);
        }

        /// <summary>
        /// Adds additional XElement and XAttribute settings to the XML representation
        /// </summary>
        /// <returns></returns>
        public override List<XObject> GetAdditionalSettings()
        {
            var settings = base.GetAdditionalSettings();
            settings.Add(new XAttribute(TYPE, QueueType));
            settings.Add(new XAttribute(INITIAL_PARAMS, InitialParams));
            return settings;
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            base.ParseFromXml(element);
            QueueType = element.TypeAttribute(TYPE, TypeResolver.Current);
            if (!typeof(INamedQueueProvider).IsAssignableFrom(QueueType))
            {
                throw new ConfigurationErrorsException(
                    String.Format("Queue provider '{0}' must implement INamedQueueProvider", QueueType));
            }
            InitialParams = element.OptionalStringAttribute(INITIAL_PARAMS);
        }

        /// <summary>
        /// Creates a new resource connection object from the settings.
        /// </summary>
        /// <returns>Newly created resource object</returns>
        public override object GetResourceConnectionFromSettings()
        {
            return string.IsNullOrWhiteSpace(InitialParams)
                ? Activator.CreateInstance(QueueType)
                : Activator.CreateInstance(QueueType, new object[] { InitialParams });
        }
    }
}