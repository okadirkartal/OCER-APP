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


        public PriceCalculator() { }

        public PriceCalculator(IRepository<Equipments> equipmentsRepository, IRepository<EquipmentTypes> equipmentTypesRepository)
        {
            _equipmentsRepository = equipmentsRepository;
            _equipmentTypesRepository = equipmentTypesRepository;
        }

        public int FindPreDefinedDay(Equipments model)
        {
            int equipmentType = (int)model.Type;

            return (from q in _equipmentTypesRepository.Table
                    where q.Id == equipmentType
                    select new
                    {
                        q.PreDefinedDay}).Single().PreDefinedDay;
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

        public virtual decimal CalculateRentalFee(Equipments equipment,int RentDay,IQueryable<RentalFeeTypes> feeTypes)
        {
            decimal resultFee = 0m;

            var preDefinedDay = FindPreDefinedDay(equipment);


            string OneTimeRentalFee = nameof(EnmFeeTypes.OneTimeRentalFee);
            string PremiumDailyFee = nameof(EnmFeeTypes.PremiumDailyFee);
            string RegularDailyFee = nameof(EnmFeeTypes.RegularDailyFee);

            switch (equipment.Type)
            {
                case (int)EnmEquipmentTypes.Heavy:
                    {
                        resultFee = feeTypes.Single(x => x.FeeType == OneTimeRentalFee).Fee +
                                    (feeTypes.Single(x => x.FeeType == PremiumDailyFee).Fee *
                                    RentDay);
                        break;
                    }
                case (int)EnmEquipmentTypes.Regular:
                    {
                        var premiumDailyFee =
                            feeTypes.Single(x => x.FeeType == PremiumDailyFee);


                        resultFee += feeTypes.Single(x => x.FeeType == OneTimeRentalFee).Fee +
                                     premiumDailyFee.Fee *(RentDay>preDefinedDay?preDefinedDay:RentDay);
                        if (RentDay > preDefinedDay)
                        {
                            resultFee += (feeTypes.Single(x => x.FeeType == RegularDailyFee).Fee *
                                          (RentDay-preDefinedDay));
                        }

                        break;
                    }
                case (int)EnmEquipmentTypes.Specialized:
                    {
                        resultFee += (feeTypes.Single(x => x.FeeType == PremiumDailyFee).Fee *
                                      (RentDay>preDefinedDay?preDefinedDay:RentDay));
                        if (RentDay > preDefinedDay)
                        {
                            resultFee += (feeTypes.Single(x => x.FeeType == RegularDailyFee).Fee *
                                          (RentDay - preDefinedDay));
                        }

                        break;
                    }
            }
            return resultFee;
        } 
    }
}
