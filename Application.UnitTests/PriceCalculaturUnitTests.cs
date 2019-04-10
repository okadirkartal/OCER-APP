using Application.Business;
using Application.Core;
using Application.Core.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.UnitTests
{
    [TestFixture]
    public class PriceCalculaturUnitTests
    {
        //UnitOfWork_InıtıialCondition_RedirectsToHome
        //UsersLogIn_WithValidCredentials_RedirectsToHome
        private Mock<IPriceCalculator> _priceCalculator;

        [SetUp]
        public void SetUp()
        {
            _priceCalculator = new Mock<IPriceCalculator>() { CallBase = true };
        }


        //Arrange -> Act -> Assert
        [Test]
        public void IsValidRentalFee_WhenAddHeavyEquipment_ReturnsTrue()
        {
            var equipment = new Equipments { Type = (int)EnmEquipmentTypes.Heavy };


            var feeTypes = new List<RentalFeeTypes>
            {
                new  RentalFeeTypes{ Fee=100,FeeType=nameof(EnmFeeTypes.OneTimeRentalFee)},
                  new  RentalFeeTypes{ Fee=60,FeeType=nameof(EnmFeeTypes.PremiumDailyFee)},
                    new  RentalFeeTypes{ Fee=40,FeeType=nameof(EnmFeeTypes.RegularDailyFee)},
            }.AsQueryable();


            _priceCalculator.Setup(x => x.FindPreDefinedDay(equipment)).Returns(0);

            var result = _priceCalculator.Object.CalculateRentalFee(equipment, 3, feeTypes);

            _priceCalculator.Verify(x => x.CalculateRentalFee(equipment, 3, feeTypes),Times.Once());
            Assert.AreEqual(280m, result);
        }

    }
}
