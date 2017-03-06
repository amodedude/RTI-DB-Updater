namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.vessel_historical")]
    public partial class vessel_historical
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sampleID { get; set; }

        [StringLength(45)]
        public string date { get; set; }

        public int? lbs_chemical { get; set; }

        [StringLength(45)]
        public string throughput { get; set; }

        [StringLength(45)]
        public string num_regens { get; set; }

        [StringLength(45)]
        public string toc { get; set; }

        [StringLength(45)]
        public string vessel_historical_customerID { get; set; }
    }
}
