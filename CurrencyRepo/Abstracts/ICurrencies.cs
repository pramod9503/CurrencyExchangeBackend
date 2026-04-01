using CurrencyRepo.Models.BackModels;

namespace CurrencyRepo.Abstracts
{
    public interface ICurrencies
    {
        //IQueryable<Currency> GetCurrencies(int page = 1, int count = 5);
        IQueryable<Currency> GetCurrencies();

        Task<Currency?> GetCurrency(int id);
    }
}
