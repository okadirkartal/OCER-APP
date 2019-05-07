using System.Threading.Tasks;
using Application.Core.Models;

namespace Application.Infrastructure.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<UserEquipments> UserEquipmentRepository { get; }

        IRepository<EquipmentTypes> EquipmentTypesRepository { get; }

        IRepository<Equipments> EquipmentsRepository { get; }

        IRepository<RentalFeeTypes> RentalFeeTypesRepository { get; }

        Task SaveAsync();
    }
}