namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.customer")]
    public partial class customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public customer()
        {
            customer_water = new HashSet<customer_water>();
        }

        [Column(TypeName = "uint")]
        public long customerID { get; set; }

        [StringLength(45)]
        public string name { get; set; }

        [StringLength(45)]
        public string company { get; set; }

        [StringLength(45)]
        public string plant { get; set; }

        [StringLength(45)]
        public string city { get; set; }

        [StringLength(45)]
        public string state { get; set; }

        public decimal? acid_price { get; set; }

        public decimal? caustic_price { get; set; }

        public int? demand { get; set; }

        [Column(TypeName = "uint")]
        public long? num_trains { get; set; }

        [StringLength(45)]
        public string date_added { get; set; }

        [StringLength(500)]
        public string notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<customer_water> customer_water { get; set; }
    }
}
