using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockChannel.Infrastructure.Extensions;
using StockChannel.UI.DataAccess;
using StockChannel.UI.Hubs;
using StockChannel.UI.Repositories;
using StockChannel.UI.Services;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;

namespace StockChannel.UI
{
    public class Startup
    {
        public IWebHostEnvironment Environment;
        
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            
            services.AddSignalR();
            
            services.AddMessagingConfiguration(Configuration, Environment.EnvironmentName);
            services.AddNetworkConfiguration(Configuration, Environment.EnvironmentName);
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageHandlerService, MessageHandlerService>();
            
            services.AddSingleton<IRabbitMQService, RabbitMQService>(); // Need a single instance so we can keep the referenced connect with RabbitMQ open

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString(AppDbContext.connectionName)));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationLifetime lifetime)
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
            lifetime.ApplicationStarted.Register(() => RegisterSignalRWithRabbitMQ(app.ApplicationServices));
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<StockChannelHub>($"/{StockChannelHub.HubName}");
            });

            app.UseAuthentication();
        }
        public void RegisterSignalRWithRabbitMQ(IServiceProvider serviceProvider)
        {
            // Connect to RabbitMQ
            var rabbitMQService = (IRabbitMQService)serviceProvider.GetService(typeof(IRabbitMQService));
            try
            {
                rabbitMQService.Connect();
            }
            catch (Exception)
            {

                //TODO: log this issue
            }
        }
    }
}