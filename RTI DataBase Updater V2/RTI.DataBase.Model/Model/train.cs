namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.train")]
    public partial class train
    {
        [Key]
        [Column(Order = 0)]
        public int trainID { get; set; }

        [StringLength(45)]
        public string name { get; set; }

        public int? number { get; set; }

        [StringLength(45)]
        public string gpm { get; set; }

        public int? num_beds_cation { get; set; }

        public int? num_beds_anion { get; set; }

        [StringLength(45)]
        public string using_manifold { get; set; }

        [StringLength(45)]
        public string regens_per_month { get; set; }

        [StringLength(45)]
        public string regen_duration { get; set; }

        [StringLength(45)]
        public string has_mixed_bed { get; set; }

        [StringLength(45)]
        public string has_historical_data { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customer_customerID { get; set; }
    }
}
