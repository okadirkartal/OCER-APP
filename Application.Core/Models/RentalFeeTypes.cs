using System.ComponentModel.DataAnnotations;

namespace Application.Core.Models
{
    public class RentalFeeTypes
    {
        [Key]
        public int Id { get; set; }

        public string FeeType { get; set; }

        public decimal Fee { get; set; }
    }
}