using Application.Core;
using Application.Core.Models;
using Application.Infrastructure.DAL;
using System.Collections.Generic;
using System.Linq;

namespace Application.Business
{
   public class PriceCalculator:IPriceCalculator
    {
        private readonly IRepository<Equipments> _equipmentsRepository;

        private readonly IRepository<EquipmentTypes>  _equipmentTypesRepository;

        private readonly IRepository<RentalFeeTypes> _rentalFeeTypesRepository;

        public PriceCalculator(IRepository<Equipments> equipmentsRepository, IRepository<EquipmentTypes> equipmentTypesRepository, IRepository<RentalFeeTypes> rentalFeeTypesRepository)
        {
            _equipmentsRepository = equipmentsRepository;
            _equipmentTypesRepository = equipmentTypesRepository;
            _rentalFeeTypesRepository = rentalFeeTypesRepository;
        }


        public int FindMinimumRentalDay(Equipments model)
        {
            int equipmentType = (int)model.Type;

            return (from q in _equipmentTypesRepository.Table
                    where q.Id == equipmentType
                    select new
                    {
                        q.MinimumRentalDay
                    }).Single().MinimumRentalDay;
        }

        public int CalculateLoyaltyPoint(List<UserEquipments> model)
        {
            var loyaltyPoint = 0;

            foreach (var item in model)
            {
                var equipment = _equipmentsRepository.Where(x => x.Id == item.EquipmentId).Single();

                loyaltyPoint += (equipment.Type == (int)EnmEquipmentTypes.Heavy) ? 2 : 1;
            }

            return loyaltyPoint;
        }


        public decimal CalculateRentalFee(UserEquipments model)
        {
            decimal resultFee = 0m;

            var equipment = _equipmentsRepository.Where(x => x.Id == model.EquipmentId).Single();

            var feeTypes = _rentalFeeTypesRepository.All();

            var minimumRentalDay = FindMinimumRentalDay(equipment);


            string OneTimeRentalFee = nameof(EnmFeeTypes.OneTimeRentalFee);
            string PremiumDailyFee = nameof(EnmFeeTypes.PremiumDailyFee);
            string RegularDailyFee = nameof(EnmFeeTypes.RegularDailyFee);

            switch (equipment.Type)
            {
                case (int)EnmEquipmentTypes.Heavy:
                    {
                        resultFee = feeTypes.Single(x => x.FeeType == OneTimeRentalFee).Fee +
                                    (feeTypes.Single(x => x.FeeType == PremiumDailyFee).Fee *
                                     model.RentDay);
                        break;
                    }
                case (int)EnmEquipmentTypes.Regular:
                    {
                        var premiumDailyFee =
                            feeTypes.Single(x => x.FeeType == PremiumDailyFee);


                        resultFee += feeTypes.Single(x => x.FeeType == OneTimeRentalFee).Fee +
                                     premiumDailyFee.Fee * minimumRentalDay;
                        if (model.RentDay > minimumRentalDay)
                        {
                            resultFee += (feeTypes.Single(x => x.FeeType == RegularDailyFee).Fee *
                                          (model.RentDay - minimumRentalDay));
                        }

                        break;
                    }
                case (int)EnmEquipmentTypes.Specialized:
                    {
                        resultFee += (feeTypes.Single(x => x.FeeType == PremiumDailyFee).Fee *
                                      minimumRentalDay);
                        if (model.RentDay > minimumRentalDay)
                        {
                            resultFee += (feeTypes.Single(x => x.FeeType == RegularDailyFee).Fee *
                                          (model.RentDay - minimumRentalDay));
                        }

                        break;
                    }
            }
            return resultFee;
        } 
    }
}
