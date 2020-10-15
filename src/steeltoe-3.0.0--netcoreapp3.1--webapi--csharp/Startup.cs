using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
{{#mongodb}}
using Steeltoe.Connector.MongoDb;
{{/mongodb}}
{{#mysql-or-mysql-efcore}}
using Steeltoe.Connector.MySql;
{{/mysql-or-mysql-efcore}}
{{#mysql-efcore}}
using Steeltoe.Connector.MySql.EFCore;
{{/mysql-efcore}}
{{#oauth}}
using Steeltoe.Connector.OAuth;
{{/oauth}}
{{#postgresql}}
using Steeltoe.Connector.PostgreSql;
{{/postgresql}}
{{#postgresql-efcore}}
using Steeltoe.Connector.PostgreSql.EFCore;
{{/postgresql-efcore}}
{{#amqp}}
using Steeltoe.Connector.RabbitMQ;
{{/amqp}}
{{#redis}}
using Steeltoe.Connector.Redis;
{{/redis}}
{{#sqlserver}}
using Steeltoe.Connector.SqlServer.EFCore;
{{/sqlserver}}
{{#eureka-client}}
using Steeltoe.Discovery.Client;
{{/eureka-client}}
{{#actuator}}
using Steeltoe.Management.CloudFoundry;
{{/actuator}}
{{#RequiresHttps}}
using Microsoft.AspNetCore.HttpsPolicy;
{{/RequiresHttps}}
{{#Auth}}
using Microsoft.AspNetCore.Authentication;
{{/Auth}}
{{#oauth}}
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
{{/oauth}}
{{#IndividualB2CAuth}}
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
{{/IndividualB2CAuth}}
{{#circuit-breaker}}
using Steeltoe.CircuitBreaker.Hystrix;
{{/circuit-breaker}}

namespace {{Namespace}}
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
{{#oauth}}
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
{{/oauth}}
{{#IndividualB2CAuth}}
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
{{/IndividualB2CAuth}}
{{#mysql}}
            services.AddMySqlConnection(Configuration);
{{/mysql}}
{{#actuator}}
            services.AddCloudFoundryActuators(Configuration);
{{/actuator}}
{{#eureka-client}}
            services.AddDiscoveryClient(Configuration);
{{/eureka-client}}
{{#postgresql}}
            services.AddPostgresConnection(Configuration);
{{/postgresql}}
{{#amqp}}
            services.AddRabbitMQConnection(Configuration);
{{/amqp}}
{{#redis}}
            // Add the Redis distributed cache.
            // We are using the Steeltoe Redis Connector to pickup the CloudFoundry
            // Redis Service binding and use it to configure the underlying RedisCache
            // This adds a IDistributedCache to the container
            services.AddDistributedRedisCache(Configuration);
            // This works like the above, but adds a IConnectionMultiplexer to the container
            // services.AddRedisConnectionMultiplexer(Configuration);
{{/redis}}
{{#mongodb}}
            services.AddMongoClient(Configuration);
{{/mongodb}}
{{#oauth}}
            services.AddOAuthServiceOptions(Configuration);
{{/oauth}}
{{#postgresql-efcore}}
            // Add Context and use Postgres as provider ... provider will be configured from VCAP_ info
            // services.AddDbContext<MyDbContext>(options => options.UseNpgsql(Configuration));
{{/postgresql-efcore}}
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
{{#RequiresHttps}}
            else
            {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
{{/RequiresHttps}}
{{#Auth}}
            app.UseAuthentication();
{{/Auth}}

{{#eureka-client}}
            app.UseDiscoveryClient();
{{/eureka-client}}
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
