using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;
        public string DefaultTopicName { get; set; } = "OdrisEventBus";
        public string EventBusConnectionString { get; set; } = string.Empty;
        public string SubscriberClientAppName { get; set; } = string.Empty;
        public string EventNamePrefix { get; set; } = string.Empty;
        public string EventNameSuffix { get; set; } = string.Empty;
        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
        public Object Connection { get; set; }
        public bool DeleteEventPrefix => !string.IsNullOrEmpty(EventNamePrefix);
        public bool DeleteEventSuffix => !string.IsNullOrEmpty(EventNameSuffix);
    }
    public enum EventBusType
    {
        RabbitMQ=0,
        AzureServisBus=1
    }
}
