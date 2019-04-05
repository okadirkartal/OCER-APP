using System.ComponentModel.DataAnnotations;

namespace Application.Core.Models
{
    public class EquipmentTypes
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }      
        
        public int MinimumRentalDay { get; set; }
    }
}