using CurrencyRepo.Models.BackModels;
using CurrencyRepo.Models.FrontModels;

namespace CurrencyRepo.Abstracts
{
    public interface IAdminCurrencies : ICurrencies
    {
        Task<Currency> AddCurrency(AddCurrencyModel model);        

        Task<Currency> UpdateCurrency(Currency currency);

        Task<Currency> UpdateRate(UpdateRateModel rateModel);

        Task<Currency> DeleteCurrency(int id);
    }
}
