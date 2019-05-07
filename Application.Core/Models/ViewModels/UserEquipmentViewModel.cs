namespace Application.Core.Models.ViewModels
{
    public class UserEquipmentViewModel
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public int EquipmentId { get; set; }

        public int EquipmentTypeId { get; set; }

        public string EquipmentTypeName { get; set; }

        public string EquipmentName { get; set; }

        public int PreDefinedDay { get; set; }

        public int UserRentalDay { get; set; }
    }
}