using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Model
{
    public class SourceConfiguration : EntityTypeConfiguration<source>
    {
        public SourceConfiguration()
        {
            Property(e => e.river)
                .IsUnicode(false);

            Property(e => e.city)
                .IsUnicode(false);

            Property(e => e.state)
                    .IsUnicode(false);

            Property(e => e.country)
                    .IsUnicode(false);

            Property(e => e.region)
                    .IsUnicode(false);

            Property(e => e.measurement_location)
                    .IsUnicode(false);

            Property(e => e.exact_lat)
                    .IsUnicode(false);

            Property(e => e.exact_lng)
                    .IsUnicode(false);

            Property(e => e.street_number)
                    .IsUnicode(false);

            Property(e => e.street_name)
                    .IsUnicode(false);

           Property(e => e.street_lat)
                    .IsUnicode(false);

            Property(e => e.street_lng)
                    .IsUnicode(false);

            Property(e => e.feature_class)
                    .IsUnicode(false);

            Property(e => e.miles_from_site)
                    .IsUnicode(false);

            Property(e => e.post_code)
                    .IsUnicode(false);

            Property(e => e.place_name)
                    .IsUnicode(false);

            Property(e => e.county_number)
                    .IsUnicode(false);

            Property(e => e.county_name)
                    .IsUnicode(false);

            Property(e => e.state_name)
                    .IsUnicode(false);

            Property(e => e.agency)
                    .IsUnicode(false);

            Property(e => e.agency_id)
                    .IsUnicode(false);

            Property(e => e.full_site_name)
                    .IsUnicode(false);

            Property(e => e.unique_site_name)
                    .IsUnicode(false);
        }
    }
}
