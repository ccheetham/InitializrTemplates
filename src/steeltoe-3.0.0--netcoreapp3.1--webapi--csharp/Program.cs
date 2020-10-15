using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
{{#azure-spring-cloud}}
using Microsoft.Azure.SpringCloud.Client;
{{/azure-spring-cloud}}
{{#actuator-or-dynamic-logger}}
using Steeltoe.Extensions.Logging.DynamicSerilog;
{{/actuator-or-dynamic-logger}}
{{#cloud-foundry}}
using Steeltoe.Common.Hosting;
{{^config-server}}
using Steeltoe.Extensions.Configuration.CloudFoundry;
{{/config-server}}
{{/cloud-foundry}}
{{#config-server}}
using Steeltoe.Extensions.Configuration.config-server;
{{/config-server}}
{{#placeholder}}
using Steeltoe.Extensions.Configuration.Placeholder;
{{/placeholder}}
{{#random-value}}
using Steeltoe.Extensions.Configuration.RandomValue;
{{/ random-value}}

namespace {{Namespace}}
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
            .Build()
            {{#any-efcore}}
            .InitializeDbContexts()
            {{/any-efcore}}
            .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(configure => configure.ValidateScopes = false)
{{#cloud-foundry}}
                .UseCloudHosting() //Enable listening on a Env provided port
{{^config-server}}
                .AddCloudFoundryConfiguration() //Add cloudfoundry environment variables as a configuration source
{{/config-server}}
{{/cloud-foundry}}
{{#config-server}}
                .Addconfig-server()
{{/config-server}}
{{#placeholder}}
                .AddPlaceholderResolver()
{{/placeholder}}
{{#random-value}}
                .ConfigureAppConfiguration((b) => b.AddRandomValueSource())
{{/random-value}}
{{#azure-spring-cloud}}
                .UseAzureSpringCloudService()
{{/azure-spring-cloud}}
{{#actuator-or-dynamic-logger}}
                .ConfigureLogging((context, builder) => builder.AddSerilogDynamicConsole())
{{/actuator-or-dynamic-logger}}
                .UseStartup<Startup>();
            return builder;
        }
    }
}
