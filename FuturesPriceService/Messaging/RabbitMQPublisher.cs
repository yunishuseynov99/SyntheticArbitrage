using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using FuturesPriceService.DTOs;

namespace FuturesPriceService.Messaging
{
    public class RabbitMQPublisher
    {
        private readonly ConnectionFactory _factory;
        private readonly Serilog.ILogger _logger;
        public RabbitMQPublisher(Serilog.ILogger logger)
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                AutomaticRecoveryEnabled = true, 
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            _logger = logger;
        }

        public async Task PublishAsync(List<PriceDto> prices)
        {
            try
            {
                using var connection = await _factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                
                await channel.QueueDeclareAsync(
                    queue: "price_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = JsonSerializer.Serialize(prices);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";
                properties.DeliveryMode = DeliveryModes.Persistent;
                properties.Expiration = "3600000";
                properties.Headers = new Dictionary<string, object>();

                
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "price_queue",
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                _logger.Information("Prices sent to queue successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error publishing prices to queue: {ex.Message}");
            }
        }
    }
}
