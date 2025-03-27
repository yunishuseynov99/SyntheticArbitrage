using FuturesPriceService.Config;
using FuturesPriceService.DTOs;
using FuturesPriceService.Interfaces;
using Microsoft.Extensions.Options;

namespace FuturesPriceService.Services
{
    public class FuturesPriceService : IFuturesPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly BinanceSettings _settings;
        private readonly Serilog.ILogger _logger;

        public FuturesPriceService(HttpClient httpClient,
                                   IOptions<BinanceSettings> settings,
                                   Serilog.ILogger logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<List<FuturesPriceDto>> GetPricesAsync()
        {
            var results = new List<FuturesPriceDto>();

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
                            _logger.Information("Fetched price for {Contract}: {Price}", contract, price);

                            results.Add(new FuturesPriceDto
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
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error fetching price for {Contract}", contract);
                }
            }

            return results;
        }
    }

}
