using CurrencyExchange.Hubs;
using CurrencyRepo.Abstracts;
using CurrencyRepo.CurrencyDb;
using CurrencyExchange.Services;
using CurrencyExchange.RabbitMq;
using CurrencyRepo.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CurrencyDbContext>(
    opts => opts.UseSqlServer(builder.Configuration["ConnectionStrings:CurrenciesDbConnection"]));
builder.Services.AddScoped<ICurrencies, CustomerCurrencies>();
builder.Services.AddSingleton<ICacheKeyStore, CacheKeysStore>();
builder.Services.AddControllers();
builder.Services.AddHostedService<RabbitMqConsumerService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache(
    opts =>
    {        
        opts.ExpirationScanFrequency = TimeSpan.FromSeconds(240);
    });
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHub<CurrencyHub>("Currency/NotifyUpdate");
app.MapControllers();

app.Run();
