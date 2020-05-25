using Microsoft.Extensions.DependencyInjection;

namespace IsUakr.MessageBroker
{
    public static class MqServiceExtensions
    {
        public static void AddMqServices(this IServiceCollection serviceCollection, string uri)
        {
            serviceCollection.AddScoped<IMqService>(x => new MqService(uri));
            serviceCollection.AddScoped<IMqManager, MqManager>();
        }
    }
}
