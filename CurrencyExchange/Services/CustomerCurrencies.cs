using CurrencyRepo.Abstracts;
using CurrencyRepo.CurrencyDb;
using Microsoft.EntityFrameworkCore;
using CurrencyRepo.Models.BackModels;

namespace CurrencyExchange.Services
{
    public class CustomerCurrencies : ICurrencies
    {
        private readonly CurrencyDbContext _context;

        public CustomerCurrencies(CurrencyDbContext context) 
        {
            _context = context;
        }

        //public IQueryable<Currency> GetCurrencies(int page = 1, int count = 5)
        public IQueryable<Currency> GetCurrencies()
        {
            try
            {
                IQueryable<Currency> currencies = _context.Currencies.Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.Country).AsNoTracking().AsQueryable();
                return currencies;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Currency?> GetCurrency(int id)
        {
            if (id == 0) throw new Exception("Currency Id cannot be 0.");
            try
            {
                Currency? currency = await _context.Currencies.Where(x => x.Id == id && x.IsDeleted == false).AsNoTracking().SingleOrDefaultAsync();
                return currency;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
