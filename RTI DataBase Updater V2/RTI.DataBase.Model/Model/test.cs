namespace RTI.DataBase.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class test : DbContext
    {
        public test()
            : base("name=test")
        {
        }

        public virtual DbSet<cond_archive> cond_archive { get; set; }
        public virtual DbSet<customer> customers { get; set; }
        public virtual DbSet<customer_water> customer_water { get; set; }
        public virtual DbSet<datum> data { get; set; }
        public virtual DbSet<resin_products> resin_products { get; set; }
        public virtual DbSet<source> sources { get; set; }
        public virtual DbSet<train> trains { get; set; }
        public virtual DbSet<vessel> vessels { get; set; }
        public virtual DbSet<vessel_historical> vessel_historical { get; set; }
        public virtual DbSet<water_data> water_data { get; set; }
        public virtual DbSet<water_data_old> water_data_old { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<cond_archive>()
                .Property(e => e.measurment_date)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.company)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.plant)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.city)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.date_added)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .Property(e => e.notes)
                .IsUnicode(false);

            modelBuilder.Entity<customer>()
                .HasMany(e => e.customer_water)
                .WithRequired(e => e.customer)
                .HasForeignKey(e => e.customer_customerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<datum>()
                .Property(e => e.measurment_date)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.manufacturer)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.model_number)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.resin_type)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.primary_type)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.group)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.teir)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.chemical_structure)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.physical_structure)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.color)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.total_capacity)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.salt_split_CAP)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.price_per_cuft)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .Property(e => e.comments)
                .IsUnicode(false);

            modelBuilder.Entity<resin_products>()
                .HasMany(e => e.vessels)
                .WithRequired(e => e.resin_products)
                .HasForeignKey(e => e.resin_data_product_id)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<source>()
                .HasMany(e => e.customer_water)
                .WithRequired(e => e.source)
                .HasForeignKey(e => e.first_sourceID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<source>()
                .HasMany(e => e.customer_water1)
                .WithRequired(e => e.source1)
                .HasForeignKey(e => e.second_sourceID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<train>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<train>()
                .Property(e => e.gpm)
                .IsUnicode(false);

            modelBuilder.Entity<train>()
                .Property(e => e.using_manifold)
                .IsUnicode(false);

            modelBuilder.Entity<train>()
                .Property(e => e.regens_per_month)
                .IsUnicode(false);

            modelBuilder.Entity<train>()
                .Property(e => e.regen_duration)
                .IsUnicode(false);

            modelBuilder.Entity<train>()
                .Property(e => e.has_mixed_bed)
                .IsUnicode(false);

            modelBuilder.Entity<train>()
                .Property(e => e.has_historical_data)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.size)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.bed_number)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.lbs_chemical)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.date_replaced)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.replacement_plan)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.throughput)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.num_regens)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.toc)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.with_degassifier)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.with_polisher)
                .IsUnicode(false);

            modelBuilder.Entity<vessel>()
                .Property(e => e.vessel_customerID)
                .IsUnicode(false);

            modelBuilder.Entity<vessel_historical>()
                .Property(e => e.date)
                .IsUnicode(false);

            modelBuilder.Entity<vessel_historical>()
                .Property(e => e.throughput)
                .IsUnicode(false);

            modelBuilder.Entity<vessel_historical>()
                .Property(e => e.num_regens)
                .IsUnicode(false);

            modelBuilder.Entity<vessel_historical>()
                .Property(e => e.toc)
                .IsUnicode(false);

            modelBuilder.Entity<vessel_historical>()
                .Property(e => e.vessel_historical_customerID)
                .IsUnicode(false);

            modelBuilder.Entity<water_data>()
                .Property(e => e.sourceid)
                .IsUnicode(false);

            modelBuilder.Entity<water_data_old>()
                .Property(e => e.sourceid)
                .IsUnicode(false);
        }
    }
}
