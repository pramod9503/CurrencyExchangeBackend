using CurrencyRepo.Infrastructure;

namespace CurrencyRepo.Models.BackModels
{    
    public class CurrencyMessageModel
    {
        public Currency? Currency { get; set; }

        public CurrencyOperationEnum Operation { get; set; }
    }
}
