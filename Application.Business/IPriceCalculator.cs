using Application.Core.Models;
using System.Collections.Generic;

namespace Application.Business
{
    public interface IPriceCalculator
    {
        int FindMinimumRentalDay(Equipments model);

        int CalculateLoyaltyPoint(List<UserEquipments> model);

        decimal CalculateRentalFee(UserEquipments model);
    }
}
