using RTI.DataBase.Model.Configuration;
using System.Data.Entity;

namespace RTI.DataBase.Model
{
    public class RtiContext : DbContext
    {
        public RtiContext()
            : base("name=RTIDBModel")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<source> Sources { get; set; }
        public virtual DbSet<water_data> WaterData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SourceConfiguration());
            modelBuilder.Configurations.Add(new WaterDataConfiguration());
        }
    }
}
