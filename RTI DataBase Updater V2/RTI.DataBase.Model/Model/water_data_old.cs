namespace RTI.DataBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rtidev.water_data_old")]
    public partial class water_data_old
    {
        [Key]
        public int dataID { get; set; }

        public int? cond { get; set; }

        public int? temp { get; set; }

        public DateTime? measurment_date { get; set; }

        [StringLength(255)]
        public string sourceid { get; set; }
    }
}
