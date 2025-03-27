using FuturesPriceService.Config;
using FuturesPriceService.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

builder.Services.AddSingleton(Log.Logger);
builder.Host.UseSerilog(); 

builder.Services.Configure<BinanceSettings>(
    builder.Configuration.GetSection("BinanceSettings"));

builder.Services.AddHttpClient<IFuturesPriceService, 
    FuturesPriceService.Services.FuturesPriceService>();

GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });
builder.Services.AddHangfire((provider, config) =>
{
    config.UseMemoryStorage();
    config.UseFilter(provider.GetRequiredService<AutomaticRetryAttribute>());
});
builder.Services.AddHangfireServer();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHangfireDashboard();

string recurringJobId = "fetch-prices-job";
RecurringJob.AddOrUpdate<IFuturesPriceService>(
    recurringJobId,
    service => service.GetPricesAsync(),
    "0 * * * *",                 
     options: new RecurringJobOptions { 
     TimeZone = TimeZoneInfo.Local,
     }
);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
