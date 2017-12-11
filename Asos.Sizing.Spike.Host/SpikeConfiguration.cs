using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Asos.Sizing.Spike.Host
{
    public class SpikeConfiguration
    {
        private IConfigurationRoot configurationRoot = null;

        public SpikeConfiguration()
        {
            configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();
        }

        public IEnumerable<SubscriptionListenerConfig> SubscriptionListeners => GetSubscriptionListenerConfiguration();

        private IEnumerable<SubscriptionListenerConfig> GetSubscriptionListenerConfiguration()
        {
            return configurationRoot.GetSection("ServiceBusConfig:SubscriptionListeners").Get<List<SubscriptionListenerConfig>>();
        }
    }
}
