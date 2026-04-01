using System.ComponentModel.DataAnnotations;

namespace CurrencyRepo.Abstracts
{
    public abstract class CurrencyBase
    {
        [Required, MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string CurrencyName { get; set; } = string.Empty;

        [Required]
        public decimal Rate { get; set; } = 0M;
    }
}
