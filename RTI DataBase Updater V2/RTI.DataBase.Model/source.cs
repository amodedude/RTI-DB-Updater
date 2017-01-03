namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.sources")]
    public partial class source
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sources_sourceID { get; set; }

        [StringLength(128)]
        public string river { get; set; }

        [StringLength(255)]
        public string city { get; set; }

        [StringLength(5)]
        public string state { get; set; }

        [StringLength(45)]
        public string country { get; set; }

        [StringLength(45)]
        public string region { get; set; }

        [StringLength(255)]
        public string measurement_location { get; set; }

        [StringLength(255)]
        public string exact_lat { get; set; }

        [StringLength(255)]
        public string exact_lng { get; set; }

        [StringLength(255)]
        public string street_number { get; set; }

        [StringLength(255)]
        public string street_name { get; set; }

        [StringLength(255)]
        public string street_lat { get; set; }

        [StringLength(255)]
        public string street_lng { get; set; }

        [StringLength(255)]
        public string feature_class { get; set; }

        [StringLength(255)]
        public string miles_from_site { get; set; }

        [StringLength(15)]
        public string post_code { get; set; }

        [StringLength(255)]
        public string place_name { get; set; }

        [StringLength(255)]
        public string county_number { get; set; }

        [StringLength(255)]
        public string county_name { get; set; }

        [StringLength(255)]
        public string state_name { get; set; }

        [StringLength(255)]
        public string agency { get; set; }

        [StringLength(255)]
        public string agency_id { get; set; }

        [StringLength(255)]
        public string full_site_name { get; set; }

        [StringLength(255)]
        public string unique_site_name { get; set; }

        public int? has_data { get; set; }
    }
}
