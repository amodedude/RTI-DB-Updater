namespace RTI.DataBase.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RTIDBModel : DbContext
    {
        public RTIDBModel()
            : base("name=RTIDBModel")
        {
        }

        public virtual DbSet<source> sources { get; set; }
        public virtual DbSet<water_data> water_data { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<source>()
                .Property(e => e.river)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.city)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.country)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.region)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.measurement_location)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.exact_lat)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.exact_lng)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.street_number)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.street_name)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.street_lat)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.street_lng)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.feature_class)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.miles_from_site)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.post_code)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.place_name)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.county_number)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.county_name)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.state_name)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.agency)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.agency_id)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.full_site_name)
                .IsUnicode(false);

            modelBuilder.Entity<source>()
                .Property(e => e.unique_site_name)
                .IsUnicode(false);

            modelBuilder.Entity<water_data>()
                .Property(e => e.measurment_date)
                .IsUnicode(false);

            modelBuilder.Entity<water_data>()
                .Property(e => e.sourceid)
                .IsUnicode(false);
        }
    }
}
