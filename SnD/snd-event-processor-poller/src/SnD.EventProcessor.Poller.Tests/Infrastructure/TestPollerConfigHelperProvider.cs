using Microsoft.Extensions.Configuration;
using SnD.EventProcessor.Poller.Contracts;
using System.Collections.Generic;

namespace SnD.EventProcessor.Poller.Tests.Infrastructure
{
    public class TestPollerConfigHelperProvider
    {
        public IPollerConfigHelper CreatePollerConfigHelper(Dictionary<string, string> inMemoryPollerConfig)
        {            
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryPollerConfig)
                .Build();
            var pollerConfigHelper = new PollerConfigHelper(configuration);

            return pollerConfigHelper;            
        }
    }
}
