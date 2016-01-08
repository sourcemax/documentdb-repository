using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SourceMax.DocumentDB.WebTests {

    public class Startup {

        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env) {

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            if (env.IsDevelopment()) {

                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            // To set the key into user-secrets, use a command like:
            //      user-secret set "ConnectionStrings:DocumentDB" "Account=myaccount;Database=mydatabase;Collection=mycollection;Key=blahblahblah+mykey+blahblahblah==;"
            // Or just put the connection string into the appsettings.json file


            // Setup the DocumentDB repository with the proper connection string
            var connectionString = this.Configuration["ConnectionStrings:DocumentDB"];
            services.AddSingleton<IRepository>(sp => new Repository(connectionString));

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}