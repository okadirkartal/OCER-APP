using System.ComponentModel.DataAnnotations;

namespace Application.Infrastructure
{
    public class Equipments
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; } 
         
    }
}