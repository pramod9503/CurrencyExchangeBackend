using CurrencyRepo.Abstracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyRepo.Models.BackModels
{
    [Table("Currencies")]
    public class Currency : CurrencyBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public string Source { get; set; } = "Client Memory";
    }    
}
