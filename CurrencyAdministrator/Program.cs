using CurrencyRepo.Abstracts;
using CurrencyRepo.CurrencyDb;
using CurrencyAdministrator.Hubs;
using CurrencyRepo.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CurrencyAdministrator.Abstract;
using CurrencyAdministrator.RabbitMq;
using CurrencyAdministrator.Services;
using CurrencyAdministrator.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CurrencyDbContext>(
    opts => opts.UseSqlServer(builder.Configuration["ConnectionStrings:CurrenciesDbConnection"], b => b.MigrationsAssembly("CurrencyAdministrator")));
builder.Services.AddScoped<IAdminCurrencies, AdminCurrencies>();
builder.Services.AddSingleton<ICacheKeyStore, CacheKeysStore>();
builder.Services.AddTransient<IDirectMessages, DirectMessages>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache(
    opts =>
    {
        opts.ExpirationScanFrequency = TimeSpan.FromSeconds(240);
    });

var app = builder.Build();
SeedCurrencies.EnsurePopulated(app);
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
