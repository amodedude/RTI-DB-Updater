namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.vessel")]
    public partial class vessel
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int vesselID { get; set; }

        public int? vessel_number { get; set; }

        [StringLength(120)]
        public string name { get; set; }

        [StringLength(45)]
        public string size { get; set; }

        [StringLength(45)]
        public string bed_number { get; set; }

        [StringLength(45)]
        public string lbs_chemical { get; set; }

        [StringLength(45)]
        public string date_replaced { get; set; }

        [StringLength(45)]
        public string replacement_plan { get; set; }

        [StringLength(45)]
        public string throughput { get; set; }

        [StringLength(45)]
        public string num_regens { get; set; }

        [StringLength(45)]
        public string toc { get; set; }

        [StringLength(45)]
        public string with_degassifier { get; set; }

        [StringLength(45)]
        public string with_polisher { get; set; }

        [StringLength(45)]
        public string vessel_customerID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int train_trainID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int resin_data_product_id { get; set; }

        public double? Salt_Split { get; set; }

        public double? price_cuft_resin { get; set; }

        public virtual resin_products resin_products { get; set; }
    }
}
