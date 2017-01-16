using System.Data.Entity.ModelConfiguration;

namespace RTI.DataBase.Model.Configuration
{
    public class WaterDataConfiguration : EntityTypeConfiguration<water_data>
    {
        public WaterDataConfiguration()
        {
            Property(e => e.sourceid)
                .IsUnicode(false);
        }
    }
}
