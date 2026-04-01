using System.ComponentModel.DataAnnotations;

namespace CurrencyRepo.Models.FrontModels
{
    public class UpdateRateModel
    {
        public UpdateRateModel() { }

        [Required]
        public int Id { get; set; }

        [Required]
        public decimal Rate { get; set; }
    }
}
