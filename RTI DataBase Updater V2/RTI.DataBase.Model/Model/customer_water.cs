namespace RTI.DataBase.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Xml.Serialization;

    [Table("rtidev.customer_water")]
    public partial class customer_water
    {
        [Key]
        [Column(Order = 0, TypeName = "uint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long customer_customerID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int first_sourceID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int second_sourceID { get; set; }

        public int? firstSourcePercentage { get; set; }

        [XmlIgnore()]
        public virtual customer customer { get; set; }

        [XmlIgnore()]
        public virtual source source { get; set; }

        [XmlIgnore()]
        public virtual source source1 { get; set; }
    }
}
