namespace Application.Core.Models.ViewModels
{
    public class UserEquipmentViewModel
    {
        public int UserId { get; set; }

        public int EquipmentId { get; set; }

        public int EquipmentTypeId { get; set; }

        public string EquipmentTypeName { get; set; }

        public string EquipmentName { get; set; }

        public int MinimumRentalDay { get; set; }

        public int UserRentalDay { get; set; }
    }
}