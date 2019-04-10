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
        private Mock<PriceCalculator> _priceCalculator;
        private IQueryable<RentalFeeTypes> feeTypes;

        [SetUp]
        public void SetUp()
        {
            _priceCalculator = new Mock<PriceCalculator>() { CallBase = true };
            feeTypes = new List<RentalFeeTypes>
            {
                new  RentalFeeTypes{ Fee=100m,FeeType=nameof(EnmFeeTypes.OneTimeRentalFee)},
                  new  RentalFeeTypes{ Fee=60m,FeeType=nameof(EnmFeeTypes.PremiumDailyFee)},
                    new  RentalFeeTypes{ Fee=40m,FeeType=nameof(EnmFeeTypes.RegularDailyFee)},
            }.AsQueryable();
        }

        //Arrange -> Act -> Assert
        [Test]
        public void IsValidRentalFee_WhenAddHeavyEquipment_ReturnsTrue()
        {
            var equipment = new Equipments { Id = 1, Type = (int)EnmEquipmentTypes.Heavy };

            _priceCalculator.Setup(x => x.FindPreDefinedDay(equipment)).Returns(0);
            var result = _priceCalculator.Object.CalculateRentalFee(equipment, 3, feeTypes);

            _priceCalculator.Verify(x => x.CalculateRentalFee(equipment, 3, feeTypes), Times.Once());
            Assert.AreEqual(280m, result);
        }

        [Test]
        public void IsValidRentalFee_WhenRegularEquipment_ReturnsTrue()
        {
            var equipment = new Equipments { Id = 1, Type = (int)EnmEquipmentTypes.Regular };

            _priceCalculator.Setup(x => x.FindPreDefinedDay(equipment)).Returns(2);
            var result = _priceCalculator.Object.CalculateRentalFee(equipment, 1, feeTypes);
            Assert.AreEqual(160m, result);

            result = _priceCalculator.Object.CalculateRentalFee(equipment, 2, feeTypes);
            Assert.AreEqual(220m, result);

            result = _priceCalculator.Object.CalculateRentalFee(equipment, 3, feeTypes);
            Assert.AreEqual(260m, result);
        }


        [Test]
        public void IsValidRentalFee_WhenSpecializedEquipment_ReturnsTrue()
        {
            var equipment = new Equipments { Id = 1, Type = (int)EnmEquipmentTypes.Specialized };

            _priceCalculator.Setup(x => x.FindPreDefinedDay(equipment)).Returns(3);
            var result = _priceCalculator.Object.CalculateRentalFee(equipment, 1, feeTypes);
            Assert.AreEqual(60m, result);

            result = _priceCalculator.Object.CalculateRentalFee(equipment, 2, feeTypes);
            Assert.AreEqual(120m, result);

            result = _priceCalculator.Object.CalculateRentalFee(equipment, 3, feeTypes);
            Assert.AreEqual(180m, result);

            result = _priceCalculator.Object.CalculateRentalFee(equipment, 4, feeTypes);
            Assert.AreEqual(220m, result);
        }

        [Test]
        public void CalculateLoyaltyPoint_WithValidEquipmentType_ReturnsTrue()
        {
            var equipments = new List<UserEquipments>
            {
                new UserEquipments {   EquipmentTypeId=(int)EnmEquipmentTypes.Heavy},
                new UserEquipments {  EquipmentTypeId=(int)EnmEquipmentTypes.Heavy},
                new UserEquipments {  EquipmentTypeId=(int)EnmEquipmentTypes.Regular},
                new UserEquipments {  EquipmentTypeId=(int)EnmEquipmentTypes.Regular},
                new UserEquipments {  EquipmentTypeId=(int)EnmEquipmentTypes.Specialized},
                new UserEquipments {  EquipmentTypeId=(int)EnmEquipmentTypes.Specialized},
            };

            var result = _priceCalculator.Object.CalculateLoyaltyPoint(equipments);
            Assert.AreEqual(8, result);

            equipments.RemoveAt(0);
            result = _priceCalculator.Object.CalculateLoyaltyPoint(equipments);
            Assert.AreEqual(6, result);
        }

    }
}
