using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
{{#Auth}}
using Microsoft.AspNetCore.Authorization;
{{/Auth}}
using Microsoft.AspNetCore.Mvc;
{{#sqlserver}}
using Microsoft.Data.SqlClient;
using System.Data;
{{/sqlserver}}
{{#mysql}}
using MySql.Data.MySqlClient;
using System.Data;
{{/mysql}}
{{#postgresql}}
using Npgsql;
using System.Data;
{{/postgresql}}
{{#mongodb}}
using MongoDB.Driver;
using System.Data;
{{/mongodb}}
{{#redis}}
using Microsoft.Extensions.Caching.Distributed;
{{/redis}}
{{#amqp}}
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
{{/amqp}}
{{#any-config-source}}
using Microsoft.Extensions.Configuration;
{{/any-config-source}}
{{#cloud-foundry}}
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Options;
{{/cloud-foundry}}
namespace {{Namespace}}.Controllers
{
    {{#Auth}}
    [Authorize]
    {{/Auth}}
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
{{^more-than-one-values-controller-with-args }}
        {{#sqlserver}}
        private readonly SqlConnection _dbConnection;
        public ValuesController([FromServices] SqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<string> tables = new List<string>();

            _dbConnection.Open();
            DataTable dt = _dbConnection.GetSchema("Tables");
            _dbConnection.Close();
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            return tables;
        }
        {{/sqlserver}}
        {{#mysql}}
        private readonly MySqlConnection _dbConnection;
        public ValuesController([FromServices] MySqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<string> tables = new List<string>();

            _dbConnection.Open();
            DataTable dt = _dbConnection.GetSchema("Tables");
            _dbConnection.Close();
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            return tables;
        }
        {{/mysql}}
        {{#postgresql}}
        private readonly NpgsqlConnection _dbConnection;
        public ValuesController([FromServices] NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<string> tables = new List<string>();

            _dbConnection.Open();
            DataTable dt = _dbConnection.GetSchema("Databases");
            _dbConnection.Close();
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            return tables;
        }
        {{/postgresql}}
        {{#mongodb}}
        private readonly IMongoClient _mongoClient;
        private readonly MongoUrl _mongoUrl;
        public ValuesController(IMongoClient mongoClient, MongoUrl mongoUrl)
        {
            _mongoClient = mongoClient;
            _mongoUrl = mongoUrl;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return _mongoClient.ListDatabaseNames().ToList();
        }
        {{/mongodb}}
        {{#redis}}
        private readonly IDistributedCache _cache;
        public ValuesController(IDistributedCache cache)
        {
            _cache = cache;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            await _cache.SetStringAsync("MyValue1", "123");
            await _cache.SetStringAsync("MyValue2", "456");
            string myval1 = await _cache.GetStringAsync("MyValue1");
            string myval2 = await _cache.GetStringAsync("MyValue2");
            return new string[]{ myval1, myval2};
        }
        {{/redis}}
        {{#amqp}}
        private readonly ILogger _logger;
        private readonly ConnectionFactory _factory;
        private const string queueName = "my-queue";
        public ValuesController(ILogger<ValuesController> logger, [FromServices] ConnectionFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //the queue
                channel.QueueDeclare(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
                // consumer
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    string msg = Encoding.UTF8.GetString(ea.Body);
                    _logger.LogInformation("Received message: " + msg);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
                // publisher
                int i = 0;
                while (i<5) { //write a message every second, for 5 seconds
                    var body = Encoding.UTF8.GetBytes($"Message {++i}");
                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                    Thread.Sleep(1000);
                }
            }
            return "Wrote 5 message to the info log. Have a look!";
        }
        {{/amqp}}
        {{#config-server}}
        private readonly IConfiguration _config;
        public ValuesController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var val1 = _config["Value1"];
            var val2 = _config["Value2"];
            return new string[] { val1, val2 };
        }
        {{/config-server}}
        {{#placeholder}}
        private readonly IConfiguration _config;
        public ValuesController(IConfiguration config)
        {
            _config = config;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var val1 = _config["ResolvedPlaceholderFromEnvVariables"];
            var val2 = _config["UnresolvedPlaceholder"];
            var val3 = _config["ResolvedPlaceholderFromJson"];
            return new string[] { val1, val2, val3 };
        }
        {{/placeholder}}
        {{#random-value}}
        private readonly IConfiguration _config;
        public ValuesController(IConfiguration config)
        {
            _config = config;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var val1 = _config["random:int"];
            var val2 = _config["random:uuid"];
            var val3 = _config["random:string"];

            return new string[] { val1, val2, val3 };
        }
        {{/random-value}}
        {{#circuit-breaker}}
         // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            MyCircuitBreakerCommand cb = new MyCircuitBreakerCommand("ThisIsMyBreaker");
            cb.IsFallbackUserDefined = true;
            string a = await cb.ExecuteAsync();
            return new string[] { a };
        }
        {{/circuit-breaker}}
        {{#cloud-foundry}}
        private readonly ILogger _logger;
        private CloudFoundryApplicationOptions _appOptions { get; set; }
        private CloudFoundryServicesOptions _serviceOptions { get; set; }
        public ValuesController(ILogger<ValuesController> logger, IOptions<CloudFoundryApplicationOptions> appOptions, IOptions<CloudFoundryServicesOptions> serviceOptions)
        {
            _logger = logger;
            _appOptions = appOptions.Value;
            _serviceOptions = serviceOptions.Value;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            string appName = _appOptions.ApplicationName;
            string appInstance = _appOptions.ApplicationId;
            /*_serviceOptions.Services["user-provided"]
                                                       .First(q => q.Name.Equals("xxxxxxx"))
                                                       .Credentials["xxxxxxx"].Value*/
            return new string[] { appInstance, appName };
        }
        {{/cloud-foundry}}
{{/more-than-one-values-controller-with-args}}
{{#more-than-one-values-controller-with-args}}
        [HttpGet]
public ActionResult<string> Get()
{
    return "value";
}
{{/more-than-one-values-controller-with-args}}
        {{^values-controller-with-args}}
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "value";
        }
        {{/values-controller-with-args}}
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
