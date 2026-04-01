using CurrencyRepo.Models.BackModels;

namespace CurrencyRepo.Abstracts
{
    public interface ICurrencyClient
    {
        Task ReceiveUpdate(CurrencyMessageModel messageModel);
    }
}
