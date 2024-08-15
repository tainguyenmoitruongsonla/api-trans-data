using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataTransmissionAPI.Data
{
    public class StoragePreData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ConstructionCode { get; set; }
        public string StationCode { get; set; }
        public string ParameterName { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Time { get; set; }
        public bool DeviceStatus { get; set; }

        [ForeignKey("ConstructionCode")]
        public virtual Construction construction { get; set; }
    }
}