using PriceDifferenceService.DTOs;
using PriceDifferenceService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PriceDifferenceService.Messaging
{
    public class RabbitMQConsumer : IHostedService
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private Task _consumingTask;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Serilog.ILogger _logger;
        private readonly IPriceDifferenceService _priceDifferenceService;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumer(Serilog.ILogger logger, IPriceDifferenceService priceDifferenceService, IServiceProvider serviceProvider)
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
            _priceDifferenceService = priceDifferenceService;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _consumingTask = StartConsumingAsync(_cancellationTokenSource.Token);

            return _consumingTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();

            if (_consumingTask != null)
            {
                await Task.WhenAny(_consumingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

           await _channel.CloseAsync();
           await _connection.CloseAsync();
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: "price_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (ch, ea) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.Information($"Received message: {message}");

                    try
                    {
                        var prices = JsonSerializer.Deserialize<List<PriceDto>>(message);
                        if (prices != null && prices.Count >= 2)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var priceDifferenceService = scope.ServiceProvider.GetRequiredService<IPriceDifferenceService>();
                                await priceDifferenceService.CalculateAndSaveDifferenceAsync(prices);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error processing message: {ex.Message}");
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                };

                string consumerTag = await _channel.BasicConsumeAsync(
                    queue: "price_queue",
                    autoAck: false,
                    consumer: consumer);

                _logger.Information("Started consuming messages...");
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error consuming messages: {ex.Message}");
            }
        }
    }
}
