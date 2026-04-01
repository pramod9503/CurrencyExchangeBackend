using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using CurrencyExchange.Hubs;
using RabbitMQ.Client.Events;
using CurrencyRepo.Abstracts;
using Microsoft.AspNetCore.SignalR;
using CurrencyRepo.Models.BackModels;
using Microsoft.Extensions.Caching.Distributed;

namespace CurrencyExchange.RabbitMq
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private IChannel _channel;
        private IConnection _connection;
        private readonly ICacheKeyStore _keyStore;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<CurrencyHub, ICurrencyClient> _hubContext;

        public RabbitMqConsumerService(IConfiguration configuration, ICacheKeyStore keyStore, 
            IDistributedCache cache, IHubContext<CurrencyHub, ICurrencyClient> hubContext) 
        {
            _cache = cache;
            _keyStore = keyStore;
            _hubContext = hubContext;
            _configuration = configuration;                                    

            var factory = new ConnectionFactory() 
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? string.Empty,  
                UserName = _configuration["RabbitMQ:UserName"] ?? string.Empty,  
                Password = _configuration["RabbitMQ:Password"] ?? string.Empty  
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = _configuration["RabbitMQ:QueueName"] ?? string.Empty; 
            await _channel.QueueDeclareAsync(queue:queueName, durable:true, 
                exclusive:false, autoDelete:false, arguments:null);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) => 
            {
                var body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                CurrencyMessageModel? messageModel = JsonSerializer.Deserialize<CurrencyMessageModel>(message, JsonSerializerOptions.Default);

                Console.WriteLine(message);
                foreach (string key in _keyStore.CacheKeys)
                {
                    await _cache.RemoveAsync(key);                    
                }
                _keyStore.Clear();
                if (messageModel != null)
                {
                    await _hubContext.Clients.All.ReceiveUpdate(messageModel);
                }
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck:false, consumer:consumer);
        }

        public override void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            base.Dispose();
        }
    }
}
