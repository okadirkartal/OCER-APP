using Application.Business;
using Application.Core;
using Application.Core.Models;
using Application.Core.Models.ViewModels;
using Application.Infrastructure.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.API.Controllers
{
    [Produces("application/json")]
    [Route("api/UserEquipments")]
    [ApiController]
    public class UserEquipmentsController : ControllerBase
    {

        private readonly IRepository<UserEquipments> _repository;

        private readonly IRepository<EquipmentTypes> _equipmentTypesRepository;

        private readonly IRepository<Equipments> _equipmentsRepository;

        private readonly IPriceCalculator _priceCalculator;

        private readonly IRepository<RentalFeeTypes> _rentalFeeTypesRepository;

        public UserEquipmentsController(IRepository<UserEquipments> repository,
            IRepository<EquipmentTypes> equipmentTypesRepository,
            IRepository<Equipments> equipmentsRepository,
            IPriceCalculator priceCalculator,
            IRepository<RentalFeeTypes> rentalFeeTypesRepository)
        {
            _repository = repository;
             _equipmentTypesRepository = equipmentTypesRepository;
            _equipmentsRepository = equipmentsRepository;
            _priceCalculator = priceCalculator;
            _rentalFeeTypesRepository = rentalFeeTypesRepository;
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
                equipments = await _equipmentsRepository.Where(x => x.Id == equipmentId).ToListAsync();
            else
                equipments = await _equipmentsRepository.All().ToListAsync();

            List<UserEquipmentViewModel> returnList = GetExtendedEquipmentViewModel(equipments);

            return returnList;
        }


        [HttpGet, Route("GenerateReport")]
        public async Task<Invoice> GenerateReport([FromHeader] int userId)
        {
            Invoice invoice = new Invoice() { Equipments = new List<InvoiceOfEquipment>() };

            var userEquipments = await _repository.Where(x => x.UserId == userId).ToListAsync();

            var equipmentTypes = _equipmentTypesRepository.All();

            foreach (var item in userEquipments)
            {
                var equipment = await _equipmentsRepository.Where(x => x.Id == item.EquipmentId).SingleAsync();

                var price =_priceCalculator.CalculateRentalFee(equipment,item.RentDay, _rentalFeeTypesRepository.All());

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

            invoice.LoyaltPoint = _priceCalculator.CalculateLoyaltyPoint(userEquipments);

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
                    PreDefinedDay = equipmentType.PreDefinedDay,
                    EquipmentTypeName = equipmentType.Name,
                    EquipmentTypeId = item.Type
                });
            }

            return returnList;
        }

       

        

       
        #endregion
    }
}
