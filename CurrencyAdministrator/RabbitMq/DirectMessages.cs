using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using CurrencyRepo.Abstracts;
using CurrencyAdministrator.Hubs;
using Microsoft.AspNetCore.SignalR;
using CurrencyRepo.Models.BackModels;
using CurrencyAdministrator.Abstract;

namespace CurrencyAdministrator.RabbitMq
{
    public class DirectMessages : IDirectMessages
    {        
        private CurrencyMessageModel _currencyMessage = new();

        private readonly IConfiguration _configuration;
        private readonly IHubContext<CurrencyHub, ICurrencyClient> _hubContext;
        

        public CurrencyMessageModel CurrencyMessage 
        { 
            get => _currencyMessage;
            set => _currencyMessage = value;
             
        }        

        public DirectMessages(IConfiguration configuration, IHubContext<CurrencyHub, ICurrencyClient> hubContext)
        {            
            _hubContext = hubContext;
            _configuration = configuration;            
        }

        public DirectMessages(CurrencyMessageModel messageModel, IConfiguration configuration,
            IHubContext<CurrencyHub, ICurrencyClient> hubContext)
        {
            _hubContext = hubContext;
            _configuration = configuration;
            _currencyMessage = messageModel;                        
        }

        public async Task SendMessageAsync() 
        {
            //Main entry point to the RabbitMQ .NET AMQP client
            var connectionFactory = new ConnectionFactory()
            {                
                HostName = _configuration["RabbitMQ:HostName"] ?? string.Empty,
                UserName = _configuration["RabbitMQ:UserName"] ?? string.Empty,
                Password = _configuration["RabbitMQ:Password"] ?? string.Empty
            };

            IConnection connection = await connectionFactory.CreateConnectionAsync();
            IChannel channel = await connection.CreateChannelAsync();
            //var properties = new BasicProperties 
            //{
            //    Persistent = false,
            //    ContentType = "application/json"
            //};
            //await _hubContext.Clients.All.ReceiveUpdate(this.CurrencyMessage);
            
            string json = JsonSerializer.Serialize(this.CurrencyMessage, JsonSerializerOptions.Default);

            byte[] data = Encoding.UTF8.GetBytes(json);            
            await channel.BasicPublishAsync(
                exchange: _configuration["RabbitMQ:ExchangeName"] ?? string.Empty, 
                routingKey: _configuration["RabbitMQ:RoutingKey"] ?? string.Empty,                
                body:data);            
        }
    }
}
