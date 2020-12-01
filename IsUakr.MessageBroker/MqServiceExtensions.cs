using Microsoft.Extensions.DependencyInjection;

namespace IsUakr.MessageBroker
{
    public static class MqServiceExtensions
    {
        public static void AddMqServices(this IServiceCollection serviceCollection, string uri)
        {
            serviceCollection.AddSingleton<IMqService>(x => new MqService(uri));
            serviceCollection.AddSingleton<IMqManager, MqManager>();
        }
    }
}
