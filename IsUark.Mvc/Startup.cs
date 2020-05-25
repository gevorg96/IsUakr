using IsUakr.DAL;
using IsUakr.MessageBroker;
using IsUakr.MessageBroker.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace IsUark.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnString = Configuration.GetConnectionString("isuark_db");
            var mqConnString = Configuration.GetConnectionString("rabbit_mq");
            var exchangeName = Configuration.GetSection("RabbitConfiguration").GetValue<string>("exchangeName");
            var queueNames = Configuration.GetSection("RabbitConfiguration").GetSection("queues").Get<IList<string>>();
            services.AddSingleton(x => new QueueInfo(exchangeName, queueNames));
            services.AddTransient(o => new NpgDbContext(dbConnString));
            services.AddMqServices(mqConnString);


            //var parcer = new Parcer(builder.ConnectionString);
            //parcer.Run();
            
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}