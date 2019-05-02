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
        [TestCase(1,EnmEquipmentTypes.Heavy,0,3,280)]
        [TestCase(1,EnmEquipmentTypes.Regular,2,1,160)]
        [TestCase(1,EnmEquipmentTypes.Specialized,3,1,60)]
        [TestCase(2,EnmEquipmentTypes.Specialized,3,2,120)]     
        [TestCase(2,EnmEquipmentTypes.Specialized,3,3,180)]       
        [TestCase(2,EnmEquipmentTypes.Specialized,3,4,220)]
        public void IsValidRentalFee_WhenAddEquipment_ReturnsTrue(int equipmentId,int equipmentType,
            int preDefinedDay,int rentDay,decimal rentalFee)
        {
            var equipment = new Equipments { Id = equipmentId, Type = (int)equipmentType };

            _priceCalculator.Setup(x => x.FindPreDefinedDay(equipment)).Returns(preDefinedDay);
            var result = _priceCalculator.Object.CalculateRentalFee(equipment, rentDay, feeTypes);

            _priceCalculator.Verify(x => x.CalculateRentalFee(equipment, rentDay, feeTypes), Times.Once());
            Assert.AreEqual(rentalFee, result);
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
