using System.Threading;

namespace PriceDifferenceService.Messaging
{

    public class RabbitMQHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a scope to resolve scoped services like RabbitMQConsumer
            using (var scope = _serviceProvider.CreateScope())
            {
                var rabbitMQConsumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer>();
                var cancellationTokenSource = new CancellationTokenSource();
                await rabbitMQConsumer.StartConsumingAsync(cancellationTokenSource.Token);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask; // Assuming RabbitMQConsumer will handle stopping internally
        }
    }
}
