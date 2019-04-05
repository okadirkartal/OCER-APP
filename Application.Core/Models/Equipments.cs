using System.ComponentModel.DataAnnotations;

namespace Application.Core.Models
{
    public class Equipments
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; } 
         
    }
}