using Application.Business;
using Application.Core.Models;
using Application.Core.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.DAL.UnitOfWork;
using Microsoft.IdentityModel.Tokens;

namespace Application.API.Controllers
{
    [Produces("application/json")]
    [Route("api/UserEquipments")]
    [ApiController]
    public class UserEquipmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPriceCalculator _priceCalculator;

        public UserEquipmentsController(IUnitOfWork unitOfWork,
            IPriceCalculator priceCalculator)
        {
            _priceCalculator = priceCalculator;
            _unitOfWork = unitOfWork;
        }


        [HttpGet, Route("UserEquipmentCount")]
        public async Task<int> UserEquipmentCount([FromHeader] int userId)
        {
            try
            {
                return await _unitOfWork.UserEquipmentRepository.Query(x => x.UserId == userId).CountAsync();
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
                await _unitOfWork.UserEquipmentRepository.InsertAsync(new UserEquipments
                {
                    Id=(_unitOfWork.UserEquipmentRepository.GetAsync(null,x=>x.OrderByDescending(y=>y.Id)).Id)+1,
                    EquipmentId = model.EquipmentId,
                    RentDay = model.UserRentalDay,
                    UserId = model.UserId,
                    OperationDate = DateTime.Now
                });
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("{InvoiceId}"), Route("RemoveEquipment/{InvoiceId}")]
        public async Task<bool> RemoveEquipment([FromHeader] int userId, [FromQuery] int InvoiceId)
        {
            try
            {
                var item = _unitOfWork.UserEquipmentRepository.GetFirstOrDefaultAsync(x =>
                    x.UserId == userId && x.Id == InvoiceId);
                if (item?.Result != null)
                {    
                    await _unitOfWork.UserEquipmentRepository.DeleteAsync(item);
                    await _unitOfWork.SaveAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{EquipmentId}"), Route("GetEquipment/{EquipmentId}")]
        public async Task<List<Equipments>> GetEquipment(int equipmentId)
        {
            if (equipmentId == 0)
                return await _unitOfWork.EquipmentsRepository.GetAsync();
            return await _unitOfWork.EquipmentsRepository.GetAsync(x => x.Id == equipmentId);
        }

        [HttpGet, Route("EquipmentList")]
        public async Task<IEnumerable<UserEquipmentViewModel>> EquipmentList([FromHeader] string userId,
            [FromHeader] int equipmentId)
        {
            List<Equipments> equipments =  await _unitOfWork.EquipmentsRepository.Query().ToListAsync();
             
            var returnList = await GetExtendedEquipmentViewModel(equipments);

            return returnList;
        }
        
        [HttpGet, Route("EquipmentListById")]
        public async Task<IEnumerable<UserEquipmentViewModel>>  EquipmentListById([FromHeader] string userId,
            [FromHeader] int equipmentId)
        {
            List<Equipments> equipments = await _unitOfWork.EquipmentsRepository.GetAsync(x => x.Id == equipmentId);

            var returnList = await GetExtendedEquipmentViewModel(equipments);

            return returnList;
        }

        [HttpGet, Route("GenerateReport")]
        public async Task<Invoice> GenerateReport([FromHeader] int userId)
        {
            Invoice invoice = new Invoice() {Equipments = new List<InvoiceOfEquipment>()};

            var userEquipments = await _unitOfWork.UserEquipmentRepository.GetAsync(x => x.UserId == userId);

            var equipmentTypes = await _unitOfWork.EquipmentTypesRepository.GetAsync();

            foreach (var item in userEquipments)
            {
                var item1 = item;
                var equipment = await _unitOfWork.EquipmentsRepository.Query(x => x.Id == item1.EquipmentId)
                    .SingleAsync();

                item.EquipmentTypeId = equipment.Type;

                var price = _priceCalculator.CalculateRentalFee(equipment, item.RentDay,
                    _unitOfWork.RentalFeeTypesRepository.Query());

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
        public async Task<List<EquipmentTypes>> EquipmentTypes()
        {
            return await _unitOfWork.EquipmentTypesRepository.GetAsync();
        }

        #region Private Methods 

        private async Task<List<UserEquipmentViewModel>> GetExtendedEquipmentViewModel(IEnumerable<Equipments> model)
        {
            var returnList = new List<UserEquipmentViewModel>();

            foreach (var item in model)
            {
                int equipmentTypeId = item.Type;
                var equipmentType =
                    await _unitOfWork.EquipmentTypesRepository.GetFirstOrDefaultAsync(x => x.Id == equipmentTypeId);

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