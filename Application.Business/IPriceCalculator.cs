using Application.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Application.Business
{
    public interface IPriceCalculator
    {
        int FindPreDefinedDay(Equipments model);

        int CalculateLoyaltyPoint(List<UserEquipments> model);

        decimal  CalculateRentalFee(Equipments equipment, int RentDay, IQueryable<RentalFeeTypes> feeTypes);
    }
}
