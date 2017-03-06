namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.resin_products")]
    public partial class resin_products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public resin_products()
        {
            vessels = new HashSet<vessel>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int resin_product_id { get; set; }

        [StringLength(45)]
        public string manufacturer { get; set; }

        [StringLength(45)]
        public string name { get; set; }

        [StringLength(45)]
        public string model_number { get; set; }

        [StringLength(45)]
        public string resin_type { get; set; }

        [StringLength(45)]
        public string primary_type { get; set; }

        [StringLength(45)]
        public string group { get; set; }

        [StringLength(45)]
        public string teir { get; set; }

        [StringLength(45)]
        public string chemical_structure { get; set; }

        [StringLength(45)]
        public string physical_structure { get; set; }

        [StringLength(45)]
        public string color { get; set; }

        [StringLength(45)]
        public string total_capacity { get; set; }

        [StringLength(45)]
        public string salt_split_CAP { get; set; }

        [StringLength(45)]
        public string price_per_cuft { get; set; }

        [StringLength(500)]
        public string comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<vessel> vessels { get; set; }
    }
}
