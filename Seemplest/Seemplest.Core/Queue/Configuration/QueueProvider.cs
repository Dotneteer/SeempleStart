using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration.ResourceConnections;

namespace Seemplest.Core.Queue.Configuration
{
    [DisplayName("Queue")]
    public class QueueProvider : ResourceConnectionProviderBase
    {
        private const string PROVIDER_KEY = "providerKey";
        private const string QUEUE_NAME = "queueName";
        private const string AUTO_CREATE = "autoCreate";

        /// <summary>
        /// Gets the resource key of the queue provider
        /// </summary>
        public string ProviderKey { get; private set; }

        /// <summary>
        /// Gets the name of the queue
        /// </summary>
        public string QueueName { get; private set; }

        /// <summary>
        /// Gets the flag indicating in the queue should be created if it does not exists
        /// </summary>
        public bool AutoCreate { get; set; }

        /// <summary>
        /// Creates a new instance using the specified provider and queue name
        /// </summary>
        /// <param name="providerKey">Provider key</param>
        /// <param name="queueName">Queue name</param>
        /// <param name="autoCreate">Should the queue be automatically created?</param>
        public QueueProvider(string providerKey, string queueName, bool autoCreate = true)
        {
            ProviderKey = providerKey;
            QueueName = queueName;
            AutoCreate = autoCreate;
        }

        /// <summary>
        /// Initializes the provider using the specified XML element
        /// </summary>
        /// <param name="element"></param>
        public QueueProvider(XElement element)
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
            settings.Add(new XAttribute(PROVIDER_KEY, ProviderKey));
            settings.Add(new XAttribute(QUEUE_NAME, QueueName));
            settings.Add(new XAttribute(AUTO_CREATE, AutoCreate));
            return settings;
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            base.ParseFromXml(element);
            ProviderKey = element.StringAttribute(PROVIDER_KEY);
            QueueName = element.StringAttribute(QUEUE_NAME);
            AutoCreate = element.OptionalBoolAttribute(AUTO_CREATE, true);
        }

        /// <summary>
        /// Creates a new resource connection object from the settings.
        /// </summary>
        /// <returns>Newly created resource object</returns>
        public override object GetResourceConnectionFromSettings()
        {
            var provider = ResourceConnectionFactory.CreateResourceConnection<INamedQueueProvider>(ProviderKey);
            var queue = provider.GetQueue(QueueName);
            if (queue == null && AutoCreate)
            {
                queue = provider.CreateQueue(QueueName);
            }
            return queue;
        }
    }
}