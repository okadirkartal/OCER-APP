using System.Collections.Generic;
using Application.Core; 

namespace Application.Infrastructure.DAL
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDBContext context)
        {
           var equipmentTypes = new List<EquipmentTypes>{
                new EquipmentTypes {Id = 1, Name = nameof(EnmEquipmentTypes.Heavy), MinimumRentalDay = 0},
                new EquipmentTypes {Id = 2, Name = nameof(EnmEquipmentTypes.Regular), MinimumRentalDay = 2},
                new EquipmentTypes {Id = 3, Name = nameof(EnmEquipmentTypes.Specialized), MinimumRentalDay = 3}};

            var rentalFeeTypes = new List<RentalFeeTypes>
            {
                new RentalFeeTypes() {Id = 1, FeeType = nameof(EnmFeeTypes.OneTimeRentalFee), Fee = 100},
                new RentalFeeTypes() {Id = 2, FeeType = nameof(EnmFeeTypes.PremiumDailyFee), Fee = 60},
                new RentalFeeTypes() {Id = 3, FeeType = nameof(EnmFeeTypes.RegularDailyFee), Fee = 40}
            };


            var equipments = new List<Equipments>
            {
                new Equipments {Id = 1, Type = (int) EnmEquipmentTypes.Heavy, Name = "Caterpillar bulldozer"},
                new Equipments {Id = 2, Type = (int) EnmEquipmentTypes.Regular, Name = "KamAZ truck"},
                new Equipments {Id = 3, Type = (int) EnmEquipmentTypes.Heavy, Name = "Komatsu crane"},
                new Equipments {Id = 4, Type = (int) EnmEquipmentTypes.Regular, Name = "Volvo steamroller"},
                new Equipments {Id = 5, Type = (int) EnmEquipmentTypes.Specialized, Name = "Bosch jackhammer"}
            };


            var users = new List<Users>
            {
                new Users {UserId = 1, UserName = "demo", Password = "demo"}
            };
            
            context.EquipmentTypes.AddRange(equipmentTypes);
            context.RentalFeeTypes.AddRange(rentalFeeTypes);
            context.Equipments.AddRange(equipments);
            context.Users.AddRange(users);
            context.SaveChangesAsync();
        }

    }
}