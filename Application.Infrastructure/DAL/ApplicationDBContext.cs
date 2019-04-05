using Application.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.DAL
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> context) : base(context)
        {
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Equipments> Equipments { get; set; }

        public DbSet<EquipmentTypes> EquipmentTypes { get; set; }

        public DbSet<UserEquipments> UserEquipments { get; set; }

        public DbSet<RentalFeeTypes> RentalFeeTypes { get; set; }
    }
}