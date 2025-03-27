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

builder.Services.AddHttpClient<IFuturesPriceService, FuturesPriceService.Services.FuturesPriceService>();


builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();
