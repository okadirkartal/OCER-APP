using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Core.Models.ViewModels;
using Application.Infrastructure;
using Application.Infrastructure.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.API.Controllers
{
    [Produces("application/json")]
    [Route("api/UserEquipments")]
    [ApiController]
    public class UserEquipmentsController : ControllerBase
    {

        private readonly IRepository<UserEquipments> _repository;

        private readonly IRepository<RentalFeeTypes> _rentalFeeTypesRepository;

        private readonly IRepository<EquipmentTypes> _equipmentTypesRepository;

        private readonly IRepository<Equipments> _equipmentsRepository;

        public UserEquipmentsController(IRepository<UserEquipments> repository,
            IRepository<RentalFeeTypes> rentalFeeTypesRepository,
            IRepository<EquipmentTypes> equipmentTypesRepository,
            IRepository<Equipments> equipmentsRepository)
        {
            _repository = repository;
            _rentalFeeTypesRepository = rentalFeeTypesRepository;
            _equipmentTypesRepository = equipmentTypesRepository;
            _equipmentsRepository = equipmentsRepository;
        }


        [HttpGet, Route("UserEquipmentCount")]
        public async Task<int> UserEquipmentCount([FromHeader] int userId)
        {
            try
            {
                return await _repository.Where(x => x.UserId == userId).CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost, Route("Add")]
        public async Task<bool> AddEquipment([FromBody] UserEquipmentViewModel model)
        {
            try
            {
                await _repository.Add(new UserEquipments
                {
                    EquipmentId = model.EquipmentId,
                    RentDay = model.UserRentalDay,
                    UserId = model.UserId,
                    OperationDate = DateTime.Now
                });

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost, Route("RemoveEquipment")]
        public async Task<bool> RemoveEquipment([FromHeader] int userId, [FromHeader] int equipmentId)
        {
            try
            {
                var item = _repository.Where(x => x.UserId == userId && x.EquipmentId == equipmentId);
                if (item.Any())
                    await _repository.Delete(item.First());

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{EquipmentId}"), Route("GetEquipment/{EquipmentId}")]
        public IEnumerable<Equipments> GetEquipment(int equipmentId)
        {
            if (equipmentId == 0)
                return _equipmentsRepository.All();
            return _equipmentsRepository.Where(x => x.Id == equipmentId);
        }


        [HttpGet, Route("EquipmentList")]
        public async Task<IEnumerable<UserEquipmentViewModel>> EquipmentList([FromHeader] string userId,
            [FromHeader] int equipmentId)
        {
            List<Equipments> equipments = null;
            if (equipmentId != 0)
                equipments = _equipmentsRepository.Where(x => x.Id == equipmentId).ToList();
            else
                equipments = _equipmentsRepository.All().ToList();

            List<UserEquipmentViewModel> returnList = GetExtendedEquipmentViewModel(equipments);

            return returnList;
        }


        [HttpGet, Route("GenerateReport")]
        public async Task<Invoice> GenerateReport([FromHeader] int userId)
        {
            Invoice invoice = new Invoice() { Equipments = new List<InvoiceOfEquipment>() };

            var userEquipments = _repository.Where(x => x.UserId == userId).ToList();

            var equipmentTypes = _equipmentTypesRepository.All();

            foreach (var item in userEquipments)
            {
                var equipment = _equipmentsRepository.Where(x => x.Id == item.EquipmentId).Single();

                var price = CalculateRentalFee(item);

                invoice.TotalPrice += price;

                var equipmentType = equipmentTypes.Single(x => x.Id == equipment.Type);

                invoice.Equipments.Add(new InvoiceOfEquipment
                {
                    InvoiceId = item.Id,
                    EquipmentName = equipment.Name,
                    EquipmentType = equipmentType.Name,
                    EquipmentRentalDay = item.RentDay,
                    EquipmentRentalFee = price,
                    InvoiceDate = item.OperationDate,
                    EquipmentId = item.EquipmentId
                });
            }

            invoice.LoyaltPoint = CalculateLoyaltyPoint(userEquipments);

            return invoice;
        }


        [HttpGet, Route("EquipmentTypes")]
        public List<EquipmentTypes> EquipmentTypes()
        {
            return _equipmentTypesRepository.All().ToList();
        }

        #region Private Methods 

        private List<UserEquipmentViewModel> GetExtendedEquipmentViewModel(IEnumerable<Equipments> model)
        {
            List<UserEquipmentViewModel> returnList = new List<UserEquipmentViewModel>();

            foreach (var item in model)
            {
                int equipmentTypeId = item.Type;
                var equipmentType = _equipmentTypesRepository.Where(x => x.Id == equipmentTypeId).Single();

                returnList.Add(new UserEquipmentViewModel
                {
                    EquipmentId = item.Id,
                    EquipmentName = item.Name,
                    MinimumRentalDay = equipmentType.MinimumRentalDay,
                    EquipmentTypeName = equipmentType.Name,
                    EquipmentTypeId = item.Type
                });
            }

            return returnList;
        }

        private int FindMinimumRentalDay(Equipments model)
        {
            int equipmentType = (int)model.Type;

            return (from q in _equipmentTypesRepository.Table
                    where q.Id == equipmentType
                    select new
                    {
                        q.MinimumRentalDay
                    }).Single().MinimumRentalDay;
        }

        private int CalculateLoyaltyPoint(List<UserEquipments> model)
        {
            var loyaltyPoint = 0;

            foreach (var item in model)
            {
                var equipment = _equipmentsRepository.Where(x => x.Id == item.EquipmentId).Single();

                loyaltyPoint += (equipment.Type == (int)EnmEquipmentTypes.Heavy) ? 2 : 1;
            }

            return loyaltyPoint;
        }

        private decimal CalculateRentalFee(UserEquipments model)
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
        #endregion
    }
}
