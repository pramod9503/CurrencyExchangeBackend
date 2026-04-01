using CurrencyRepo.Models.BackModels;

namespace CurrencyAdministrator.Abstract
{
    public interface IDirectMessages
    {        
        CurrencyMessageModel CurrencyMessage { get; set; }
        
        Task SendMessageAsync();
    }
}
