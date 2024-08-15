using DataTransmissionAPI.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataTransmissionAPI.Data
{
    public class DatabaseContext : IdentityDbContext<AspNetUsers, AspNetRoles, string>
    {
        private readonly IConfiguration configuration;
        public DatabaseContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
        //
        #region DbSet

        //For Authoright -- assign permission folow dashboard
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<Dashboards> Dashboards { get; set; }
        public DbSet<UserDashboards> UserDashboards { get; set; }
        public DbSet<RoleDashboards> RoleDashboards { get; set; }
        public DbSet<Functions> Functions { get; set; }
        public DbSet<StoragePreData> StoragePreData { get; set; }
        public DbSet<Construction> Construction { get; set; }


        //Other database

        public DbSet<Station> Station { get; set; }
        public DbSet<WaterLevelData> WaterLevelData { get; set; }

        #endregion
    }
}
