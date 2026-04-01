using CurrencyRepo.Abstracts;
using CurrencyRepo.CurrencyDb;
using Microsoft.EntityFrameworkCore;
using CurrencyRepo.Models.BackModels;
using CurrencyRepo.Models.FrontModels;

namespace CurrencyAdministrator.Services
{
    public class AdminCurrencies : IAdminCurrencies
    {
        private readonly CurrencyDbContext _context;
        
        public AdminCurrencies(CurrencyDbContext context) 
        {
            _context = context;    
        }

        public async Task<Currency> AddCurrency(AddCurrencyModel model)
        {
            Currency currency = new ()
            {
                Country = model.Country,
                CurrencyName = model.CurrencyName,
                Rate = model.Rate,
                IsDeleted = false,
            };
            try
            {
                _context.Currencies.Add(currency);
                await _context.SaveChangesAsync();
                return currency;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<Currency> DeleteCurrency(int id)
        {
            if (id == 0) throw new Exception("Currency Id cannot be 0.");
            Currency? currency = await _context.Currencies.Where(x => x.Id == id && x.IsDeleted == false).AsNoTracking().SingleOrDefaultAsync();
            if (currency == null) throw new Exception("Could not found the currency record.");
            currency.IsDeleted = true;
            try
            {
                _context.Currencies.Update(currency);
                await _context.SaveChangesAsync();
                return currency;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        //public IQueryable<Currency> GetCurrencies(int page = 1, int count = 5)
        public IQueryable<Currency> GetCurrencies()
        {
            try
            {
                //IQueryable<Currency> currencies = _context.Currencies.Where(x => x.IsDeleted == false)
                //    .OrderBy(x => x.Country).Skip((page - 1) * count).Take(count).AsNoTracking().AsQueryable();
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

        public async Task<Currency> UpdateCurrency(Currency currency)
        {
            Currency? exCurrency = await _context.Currencies.Where(x => x.Id == currency.Id && x.IsDeleted == false).SingleOrDefaultAsync();
            if (exCurrency == null) throw new Exception("Could not found the currency record.");
            exCurrency.CurrencyName = currency.CurrencyName;
            exCurrency.Country = currency.Country;
            exCurrency.Rate = currency.Rate;
            try
            {
                _context.Update(exCurrency);
                await _context.SaveChangesAsync();
                return exCurrency;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Currency> UpdateRate(UpdateRateModel rateModel)
        {
            Currency? currency = await _context.Currencies.Where(x => x.Id == rateModel.Id && x.IsDeleted == false).SingleOrDefaultAsync();
            if (currency == null) throw new Exception("Could not found the currency record.");
            currency.Rate = rateModel.Rate;
            try
            {
                _context.Update(currency);
                await _context.SaveChangesAsync();
                return currency;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
