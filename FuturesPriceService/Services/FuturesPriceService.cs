using FuturesPriceService.Config;
using FuturesPriceService.DTOs;
using FuturesPriceService.Interfaces;
using FuturesPriceService.Messaging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace FuturesPriceService.Services
{
    public class FuturesPriceService : IFuturesPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly BinanceSettings _settings;
        private readonly RabbitMQPublisher _publisher;
        private readonly Serilog.ILogger _logger;
        private List<PriceDto>? _lastSuccessfulPrices;

        public FuturesPriceService(HttpClient httpClient,
                                   IOptions<BinanceSettings> settings,
                                   Serilog.ILogger logger, RabbitMQPublisher publisher)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _lastSuccessfulPrices = new List<PriceDto>();
            _publisher = publisher;
        }

        public async Task<List<PriceDto>> GetPricesAsync()
        {
            var results = new List<PriceDto>();

            foreach (var contract in _settings.Contracts)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{_settings.BaseUrl}{contract}");

                    if (response.IsSuccessStatusCode)
                    {
                        var priceData = await response.Content.ReadFromJsonAsync<PriceResponseDto>();

                        if (priceData != null)
                        {
                            decimal price = decimal.Parse(priceData.Price);
                            Console.WriteLine($"[{DateTime.Now}] - Contarct & Price Fetched: {contract} {price}");
                            _logger.Information("Fetched price for {Contract}: {Price}", contract, price);

                            results.Add(new PriceDto
                            {
                                Contract = contract,
                                Price = price,
                                Timestamp = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            _logger.Warning("No price data available for {Contract}", contract);
                        }
                    }
                    else
                    {
                        _logger.Warning("Failed to get price for {Contract}. Status: {StatusCode}",
                                         contract, response.StatusCode);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error fetching price for {Contract}", contract);
                }
            }

            if (results.Count == 0)
            {
                if (_lastSuccessfulPrices != null && _lastSuccessfulPrices.Count > 0)
                {
                    await _publisher.PublishAsync(_lastSuccessfulPrices);
                    _logger.Warning("No new data fetched. Using last successful prices.");
                    return _lastSuccessfulPrices;
                }
                else
                {
                    await _publisher.PublishAsync(new List<PriceDto>());
                    _logger.Error("No data available, and no previous data exists.");
                    return new List<PriceDto>();
                }
            }

            _lastSuccessfulPrices = results;
            await _publisher.PublishAsync(results);
            _logger.Information("Successfully retrieved prices.");
            return results;
        }
        public List<PriceDto>? GetLastSuccessfulPrices()
        {
            return _lastSuccessfulPrices;
        }

    }

}
