using CurrencyRepo.CurrencyDb;
using Microsoft.EntityFrameworkCore;
using CurrencyRepo.Models.BackModels;

namespace CurrencyAdministrator.Infrastructure
{
    public static class SeedCurrencies
    {
        public static void EnsurePopulated(IApplicationBuilder app) 
        {
            CurrencyDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<CurrencyDbContext>();
            if (context.Database.GetPendingMigrations().Any()) 
            {
                context.Database.Migrate();
            }
            if (!context.Currencies.Any()) 
            {
                context.Currencies.AddRange(
                    new Currency { Country = "USA", CurrencyName = "Dollar", Rate = 93.761636M },
                    new Currency { Country = "EU", CurrencyName = "Euro", Rate = 108.253752M },
                    new Currency { Country = "Britain", CurrencyName = "British Pound", Rate = 125.287318M },
                    new Currency { Country = "Switzerland", CurrencyName = "Swiss Franc", Rate = 118.930283M },
                    new Currency { Country = "Malaysia", CurrencyName = "Ringgit", Rate = 23.807424M },
                    new Currency { Country = "Japan", CurrencyName = "Yen", Rate = 0.589873M },
                    new Currency { Country = "China", CurrencyName = "Yuan Renminbi", Rate = 13.605083M },
                    new Currency { Country = "Bahrain", CurrencyName = "Bahraini Dinar", Rate = 249.366054M },
                    new Currency { Country = "Botswana", CurrencyName = "Pula", Rate = 6.873769M },
                    new Currency { Country = "Brazil", CurrencyName = "Brazilian Real", Rate = 17.747331M }
                    );
                context.SaveChanges();
            }
        }
    }
}
