using System;
using System.Threading.Tasks;
using Application.Core.Models;

namespace Application.Infrastructure.DAL.UnitOfWork
{
    public   class UnitOfWork :IUnitOfWork,IDisposable
    {
        private readonly ApplicationDBContext _context;

        private IRepository<UserEquipments> _userEquipmentRepository;

        private IRepository<EquipmentTypes> _equipmentTypesRepository;

        private IRepository<Equipments> _equipmentsRepository;

        private IRepository<RentalFeeTypes> _rentalFeeTypesRepository;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public IRepository<UserEquipments> UserEquipmentRepository => _userEquipmentRepository ??
                                                                       (_userEquipmentRepository = new Repository<UserEquipments>(_context));
        
        public IRepository<EquipmentTypes> EquipmentTypesRepository => _equipmentTypesRepository ??
                                                                       (_equipmentTypesRepository = new Repository<EquipmentTypes>(_context));
        
        public IRepository<Equipments> EquipmentsRepository => _equipmentsRepository ??
                                                                       (_equipmentsRepository = new Repository<Equipments>(_context));
        
        public IRepository<RentalFeeTypes> RentalFeeTypesRepository => _rentalFeeTypesRepository ?? (_rentalFeeTypesRepository=new Repository<RentalFeeTypes>(_context));
       
        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            this._disposed = true;
        }

        public void Dispose()
        {
           Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}