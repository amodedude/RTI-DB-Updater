namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.cond_archive")]
    public partial class cond_archive
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dataID { get; set; }

        public int? cond { get; set; }

        public int? temp { get; set; }

        [StringLength(45)]
        public string measurment_date { get; set; }

        public int? sourceID { get; set; }
    }
}
