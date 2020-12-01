using IsUakr.MessageBroker;
using IsUakr.MessageHandler.DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IsUakr.MessageHandler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var rabbitConn = hostContext.Configuration.GetSection("ConnectionStrings:rabbit_mq").Value;
                    var dbConn = hostContext.Configuration.GetSection("ConnectionStrings:IsUakr_db").Value;

                    services.AddSingleton(x => new ConnStrProvider("queue0", dbConn, rabbitConn));
                    services.AddSingleton<IMqService>(x => new MqService(rabbitConn));
                    services.AddSingleton<IDecisionMaker>(x => new FlatDecisionMaker(dbConn));
                    services.AddSingleton<IProcessor, MessageProcessor>();
                    services.AddHostedService<Worker>();
                });
    }
}
