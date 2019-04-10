using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Core.Models
{ 
    public class UserEquipments
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int EquipmentId { get; set; }

        public int EquipmentTypeId { get; set; }

        public int RentDay { get; set; } 
        
        public DateTime OperationDate { get; set; }
         
    }
}