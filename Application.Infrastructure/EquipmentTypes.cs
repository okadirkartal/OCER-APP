using System.ComponentModel.DataAnnotations;

namespace Application.Infrastructure
{
    public class EquipmentTypes
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }      
        
        public int MinimumRentalDay { get; set; }
    }
}